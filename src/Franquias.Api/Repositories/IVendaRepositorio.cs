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

    /// <summary>
    /// Soma o valor total das vendas confirmadas de uma unidade em um intervalo de datas,
    /// inclusivo nas duas pontas e comparado em UTC. Vendas pendentes e canceladas não
    /// compõem faturamento. Sem vendas no intervalo, devolve zero.
    /// </summary>
    Task<decimal> SomarFaturamentoConfirmadoAsync(
        int unidadeFranqueadaId,
        DateOnly periodoInicio,
        DateOnly periodoFim,
        CancellationToken cancellationToken = default);
}
