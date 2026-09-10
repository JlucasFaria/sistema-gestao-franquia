using Franquias.Api.Common;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Franqueadoras;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IFranqueadoraService"/>.
/// </summary>
public sealed class FranqueadoraService(IRepositorio<Franqueadora> franqueadoras) : IFranqueadoraService
{
    /// <inheritdoc />
    public async Task<PagedResult<FranqueadoraResponse>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default)
    {
        var pagina = await franqueadoras.ListarPaginadoAsync(parametros, cancellationToken);

        return pagina.Converter(FranqueadoraResponse.De);
    }

    /// <inheritdoc />
    public async Task<FranqueadoraResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var franqueadora = await BuscarOuFalharAsync(id, cancellationToken);

        return FranqueadoraResponse.De(franqueadora);
    }

    /// <inheritdoc />
    public async Task<FranqueadoraResponse> CriarAsync(
        CriarFranqueadoraRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var cnpj = requisicao.Cnpj.SomenteDigitos();

        if (await franqueadoras.ExisteAsync(franqueadora => franqueadora.Cnpj == cnpj, cancellationToken))
        {
            throw new ConflitoException("franqueadora", "CNPJ", cnpj);
        }

        var nova = new Franqueadora(
            requisicao.RazaoSocial,
            requisicao.NomeFantasia,
            cnpj,
            requisicao.Email,
            requisicao.Telefone,
            requisicao.Endereco.ParaEntidade());

        await franqueadoras.AdicionarAsync(nova, cancellationToken);
        await franqueadoras.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoraResponse.De(nova);
    }

    /// <inheritdoc />
    public async Task<FranqueadoraResponse> AtualizarAsync(
        int id,
        AtualizarFranqueadoraRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var franqueadora = await BuscarOuFalharAsync(id, cancellationToken);

        franqueadora.Atualizar(
            requisicao.RazaoSocial,
            requisicao.NomeFantasia,
            requisicao.Email,
            requisicao.Telefone,
            requisicao.Endereco.ParaEntidade());

        // A entidade já está rastreada: o change tracker detecta a troca do endereço, que é
        // um tipo owned. Chamar Update aqui reanexaria o grafo e disputaria a instância
        // antiga do endereço com a nova.
        await franqueadoras.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoraResponse.De(franqueadora);
    }

    private async Task<Franqueadora> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await franqueadoras.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Franqueadora", id);
}
