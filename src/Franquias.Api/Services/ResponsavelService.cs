using Franquias.Api.Common;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Responsaveis;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IResponsavelService"/>. Toda alteração passa pelo agregado
/// <see cref="UnidadeFranqueada"/>, dono da coleção de responsáveis.
/// </summary>
public sealed class ResponsavelService(IUnidadeRepositorio unidades) : IResponsavelService
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ResponsavelResponse>> ListarAsync(
        int unidadeId,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarUnidadeOuFalharAsync(unidadeId, cancellationToken);

        return [.. unidade.Responsaveis.OrderBy(responsavel => responsavel.Nome).Select(ResponsavelResponse.De)];
    }

    /// <inheritdoc />
    public async Task<ResponsavelResponse> AdicionarAsync(
        int unidadeId,
        CriarResponsavelRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarUnidadeOuFalharAsync(unidadeId, cancellationToken);

        if (unidade.Situacao == SituacaoUnidade.Encerrada)
        {
            throw new RegraDeNegocioException(
                "A unidade está encerrada e não recebe novos responsáveis.");
        }

        var cpf = requisicao.Cpf.SomenteDigitos();

        // A mesma pessoa pode responder por várias unidades, mas não duas vezes pela mesma.
        if (unidade.Responsaveis.Any(responsavel => responsavel.Cpf == cpf))
        {
            throw new ConflitoException("responsável nesta unidade", "CPF", cpf);
        }

        var novo = new Responsavel(
            requisicao.Nome,
            cpf,
            requisicao.Cargo,
            requisicao.Email,
            requisicao.Telefone,
            unidade.Id);

        unidade.AdicionarResponsavel(novo);
        await unidades.SalvarAlteracoesAsync(cancellationToken);

        return ResponsavelResponse.De(novo);
    }

    /// <inheritdoc />
    public async Task<ResponsavelResponse> AtualizarAsync(
        int unidadeId,
        int responsavelId,
        AtualizarResponsavelRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarUnidadeOuFalharAsync(unidadeId, cancellationToken);
        var responsavel = BuscarResponsavelOuFalhar(unidade, responsavelId);

        responsavel.Atualizar(requisicao.Nome, requisicao.Cargo, requisicao.Email, requisicao.Telefone);
        await unidades.SalvarAlteracoesAsync(cancellationToken);

        return ResponsavelResponse.De(responsavel);
    }

    /// <inheritdoc />
    public async Task RemoverAsync(
        int unidadeId,
        int responsavelId,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarUnidadeOuFalharAsync(unidadeId, cancellationToken);
        var responsavel = BuscarResponsavelOuFalhar(unidade, responsavelId);

        // Como a chave estrangeira é obrigatória, retirar o responsável da coleção faz o EF
        // excluí-lo: um responsável sem unidade não tem significado no domínio.
        unidade.RemoverResponsavel(responsavel);
        await unidades.SalvarAlteracoesAsync(cancellationToken);
    }

    private async Task<UnidadeFranqueada> BuscarUnidadeOuFalharAsync(
        int unidadeId,
        CancellationToken cancellationToken) =>
        await unidades.ObterPorIdAsync(unidadeId, cancellationToken)
            ?? throw new NaoEncontradoException("Unidade franqueada", unidadeId);

    /// <summary>
    /// Procura o responsável dentro da coleção da unidade informada na rota. Um
    /// identificador de responsável de outra unidade é tratado como inexistente aqui,
    /// impedindo alterar o responsável de uma loja pela rota de outra.
    /// </summary>
    private static Responsavel BuscarResponsavelOuFalhar(UnidadeFranqueada unidade, int responsavelId) =>
        unidade.Responsaveis.FirstOrDefault(responsavel => responsavel.Id == responsavelId)
            ?? throw new NaoEncontradoException(
                $"Responsável com identificador {responsavelId} não foi encontrado na unidade {unidade.Id}.");
}
