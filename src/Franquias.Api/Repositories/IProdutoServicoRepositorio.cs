using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados dos itens do catálogo — produtos e serviços.
/// </summary>
public interface IProdutoServicoRepositorio : IRepositorio<ProdutoServico>
{
    /// <summary>
    /// Lista os itens de forma paginada, com a categoria carregada, aplicando os filtros
    /// informados.
    /// </summary>
    Task<PagedResult<ProdutoServico>> ListarAsync(
        QueryParams parametros,
        FiltroProdutosRequest filtro,
        CancellationToken cancellationToken = default);
}
