using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Vendas;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados das vendas e de seus itens.
/// </summary>
public interface IVendaRepositorio : IRepositorio<Venda>
{
    /// <summary>
    /// Lista as vendas de forma paginada, com unidade e itens carregados, filtrando por
    /// unidade, intervalo de datas e estágio.
    /// </summary>
    Task<PagedResult<Venda>> ListarAsync(
        QueryParams parametros,
        FiltroVendasRequest filtro,
        CancellationToken cancellationToken = default);
}
