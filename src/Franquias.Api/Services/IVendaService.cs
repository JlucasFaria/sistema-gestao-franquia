using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Vendas;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio das vendas registradas pelas unidades franqueadas.
/// </summary>
public interface IVendaService
{
    /// <summary>
    /// Lista as vendas de forma paginada, filtrando por unidade, intervalo de datas e estágio.
    /// </summary>
    Task<PagedResult<VendaResponse>> ListarAsync(
        QueryParams parametros,
        FiltroVendasRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>Busca uma venda pelo identificador, com seus itens.</summary>
    Task<VendaResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra uma venda com pelo menos um item, vinculada à unidade informada. A venda
    /// nasce pendente: o estoque só é baixado na confirmação.
    /// </summary>
    Task<VendaResponse> RegistrarAsync(
        CriarVendaRequest requisicao,
        CancellationToken cancellationToken = default);
}
