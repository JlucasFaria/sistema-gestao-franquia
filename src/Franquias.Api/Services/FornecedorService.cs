using Franquias.Api.Common;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Fornecedores;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IFornecedorService"/>.
/// </summary>
public sealed class FornecedorService(IFornecedorRepositorio fornecedores) : IFornecedorService
{
    /// <inheritdoc />
    public async Task<PagedResult<FornecedorResponse>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default)
    {
        var pagina = await fornecedores.ListarAsync(parametros, cancellationToken);

        return pagina.Converter(FornecedorResponse.De);
    }

    /// <inheritdoc />
    public async Task<FornecedorResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarOuFalharAsync(id, cancellationToken);

        return FornecedorResponse.De(fornecedor);
    }

    /// <inheritdoc />
    public async Task<FornecedorResponse> CriarAsync(
        CriarFornecedorRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var cnpj = requisicao.Cnpj.SomenteDigitos();

        if (await fornecedores.ExisteComCnpjAsync(cnpj, idIgnorado: null, cancellationToken))
        {
            throw new ConflitoException("fornecedor", "CNPJ", cnpj);
        }

        var fornecedor = new Fornecedor(
            requisicao.RazaoSocial,
            requisicao.NomeFantasia,
            cnpj,
            requisicao.Email,
            requisicao.Telefone,
            requisicao.Endereco.ParaEntidade());

        await fornecedores.AdicionarAsync(fornecedor, cancellationToken);
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorResponse.De(fornecedor);
    }

    /// <inheritdoc />
    public async Task<FornecedorResponse> AtualizarAsync(
        int id,
        AtualizarFornecedorRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarOuFalharAsync(id, cancellationToken);

        fornecedor.Atualizar(
            requisicao.RazaoSocial,
            requisicao.NomeFantasia,
            requisicao.Email,
            requisicao.Telefone,
            requisicao.Endereco.ParaEntidade());

        // Entidade rastreada: a troca do endereço owned é detectada pelo change tracker.
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorResponse.De(fornecedor);
    }

    /// <inheritdoc />
    public async Task<FornecedorResponse> AtivarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarOuFalharAsync(id, cancellationToken);

        fornecedor.Ativar();
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorResponse.De(fornecedor);
    }

    /// <inheritdoc />
    public async Task<FornecedorResponse> InativarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarOuFalharAsync(id, cancellationToken);

        fornecedor.Inativar();
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorResponse.De(fornecedor);
    }

    /// <inheritdoc />
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var fornecedor = await fornecedores.ObterComProdutosAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Fornecedor", id);

        // O vínculo está configurado em cascata, então o banco não recusaria a exclusão:
        // ele apagaria em silêncio todo o histórico de preços e prazos negociados. A trava
        // aqui torna essa perda uma decisão explícita, e não um efeito colateral.
        if (fornecedor.Produtos.Count > 0)
        {
            throw new RegraDeNegocioException(
                $"O fornecedor '{fornecedor.NomeFantasia}' possui {fornecedor.Produtos.Count} "
                + "produto(s) homologado(s) e não pode ser excluído. Inative-o para impedir "
                + "novas homologações preservando o histórico.");
        }

        fornecedores.Remover(fornecedor);
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);
    }

    private async Task<Fornecedor> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await fornecedores.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Fornecedor", id);
}
