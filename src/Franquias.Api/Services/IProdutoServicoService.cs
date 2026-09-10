using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio dos itens do catálogo — produtos e serviços.
/// </summary>
public interface IProdutoServicoService
{
    /// <summary>Lista os itens de forma paginada, aplicando os filtros informados.</summary>
    Task<PagedResult<ProdutoResponse>> ListarAsync(
        QueryParams parametros,
        FiltroProdutosRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>Busca um item pelo identificador.</summary>
    Task<ProdutoResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cadastra um item em uma categoria ativa. O item nasce com status Ativo.
    /// </summary>
    Task<ProdutoResponse> CriarAsync(
        CriarProdutoRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Altera categoria, nome, descrição e preço base de um item. A natureza — produto ou
    /// serviço — não é alterável.
    /// </summary>
    Task<ProdutoResponse> AtualizarAsync(
        int id,
        AtualizarProdutoRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Altera a situação do item no catálogo. Item descontinuado não volta ao catálogo.
    /// </summary>
    Task<ProdutoResponse> AlterarStatusAsync(
        int id,
        StatusProduto status,
        CancellationToken cancellationToken = default);

    /// <summary>Reativa o cadastro de um item. Operação idempotente.</summary>
    Task<ProdutoResponse> AtivarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inativa o cadastro de um item, sem removê-lo: vendas e estoque continuam vinculados
    /// a ele. Operação idempotente.
    /// </summary>
    Task<ProdutoResponse> InativarAsync(int id, CancellationToken cancellationToken = default);
}
