using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IProdutoServicoService"/>.
/// </summary>
public sealed class ProdutoServicoService(
    IProdutoServicoRepositorio produtos,
    IRepositorio<Categoria> categorias) : IProdutoServicoService
{
    /// <inheritdoc />
    public async Task<PagedResult<ProdutoResponse>> ListarAsync(
        QueryParams parametros,
        FiltroProdutosRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var pagina = await produtos.ListarAsync(parametros, filtro, cancellationToken);

        return pagina.Converter(ProdutoResponse.De);
    }

    /// <inheritdoc />
    public async Task<ProdutoResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var produto = await BuscarOuFalharAsync(id, cancellationToken);

        return ProdutoResponse.De(produto);
    }

    /// <inheritdoc />
    public async Task<ProdutoResponse> CriarAsync(
        CriarProdutoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var categoria = await BuscarCategoriaAtivaOuFalharAsync(requisicao.CategoriaId, cancellationToken);

        var produto = new ProdutoServico(
            categoria.Id,
            requisicao.Nome.Trim(),
            requisicao.Descricao,
            requisicao.PrecoBase,
            requisicao.EhServico);

        await produtos.AdicionarAsync(produto, cancellationToken);
        await produtos.SalvarAlteracoesAsync(cancellationToken);

        return ProdutoResponse.De(await BuscarOuFalharAsync(produto.Id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<ProdutoResponse> AtualizarAsync(
        int id,
        AtualizarProdutoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var produto = await BuscarOuFalharAsync(id, cancellationToken);

        // A categoria atual é aceita mesmo que tenha sido inativada depois do cadastro: o
        // item precisa continuar editável. Só a mudança para outra categoria exige que ela
        // esteja ativa.
        if (requisicao.CategoriaId != produto.CategoriaId)
        {
            await BuscarCategoriaAtivaOuFalharAsync(requisicao.CategoriaId, cancellationToken);
        }

        produto.Atualizar(
            requisicao.CategoriaId,
            requisicao.Nome.Trim(),
            requisicao.Descricao,
            requisicao.PrecoBase);

        await produtos.SalvarAlteracoesAsync(cancellationToken);

        return ProdutoResponse.De(await BuscarOuFalharAsync(id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<ProdutoResponse> AlterarStatusAsync(
        int id,
        StatusProduto status,
        CancellationToken cancellationToken = default)
    {
        var produto = await BuscarOuFalharAsync(id, cancellationToken);

        // Descontinuado significa retirado do catálogo em definitivo. Um item que volta ao
        // mercado com outra formulação ou outro preço de referência é um item novo.
        if (produto.Status == StatusProduto.Descontinuado && status != StatusProduto.Descontinuado)
        {
            throw new RegraDeNegocioException(
                $"O item '{produto.Nome}' foi descontinuado e não pode voltar ao catálogo. "
                + "Cadastre-o novamente como um novo item.");
        }

        if (produto.Status != status)
        {
            produto.AlterarStatus(status);
            await produtos.SalvarAlteracoesAsync(cancellationToken);
        }

        return ProdutoResponse.De(produto);
    }

    /// <inheritdoc />
    public async Task<ProdutoResponse> AtivarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var produto = await BuscarOuFalharAsync(id, cancellationToken);

        produto.Ativar();
        await produtos.SalvarAlteracoesAsync(cancellationToken);

        return ProdutoResponse.De(produto);
    }

    /// <inheritdoc />
    public async Task<ProdutoResponse> InativarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var produto = await BuscarOuFalharAsync(id, cancellationToken);

        produto.Inativar();
        await produtos.SalvarAlteracoesAsync(cancellationToken);

        return ProdutoResponse.De(produto);
    }

    private async Task<ProdutoServico> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await produtos.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Produto ou serviço", id);

    private async Task<Categoria> BuscarCategoriaAtivaOuFalharAsync(
        int categoriaId,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.ObterPorIdAsync(categoriaId, cancellationToken)
            ?? throw new NaoEncontradoException("Categoria", categoriaId);

        if (!categoria.Ativo)
        {
            throw new RegraDeNegocioException(
                $"A categoria '{categoria.Nome}' está inativa e não aceita novos itens.");
        }

        return categoria;
    }
}
