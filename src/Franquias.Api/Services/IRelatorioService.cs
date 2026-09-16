using Franquias.Api.DTOs.Relatorios;

namespace Franquias.Api.Services;

/// <summary>
/// Relatórios e indicadores gerenciais da rede.
/// </summary>
public interface IRelatorioService
{
    /// <summary>
    /// Faturamento confirmado da rede no período, aberto por unidade, da maior para a menor.
    /// </summary>
    Task<RelatorioFaturamentoResponse> FaturamentoPorUnidadeAsync(
        FiltroPeriodoRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ranking das unidades por faturamento confirmado no período, com a participação de
    /// cada uma no total da rede.
    /// </summary>
    /// <param name="filtro">Intervalo de datas considerado.</param>
    /// <param name="limite">Quantas colocações devolver; nulo devolve todas as unidades.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<IReadOnlyList<RankingUnidadeResponse>> RankingPorFaturamentoAsync(
        FiltroPeriodoRequest filtro,
        int? limite = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Royalties gerados no período, com o total da rede e a abertura por unidade. Uma
    /// cobrança entra no relatório quando todo o seu período de apuração cabe no intervalo.
    /// </summary>
    Task<RelatorioRoyaltiesResponse> RoyaltiesGeradosAsync(
        FiltroPeriodoRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Produtos e serviços mais vendidos no período, da maior para a menor quantidade.
    /// </summary>
    /// <param name="filtro">Intervalo de datas considerado.</param>
    /// <param name="limite">Quantas colocações devolver; nulo devolve todos os itens vendidos.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<IReadOnlyList<ProdutoMaisVendidoResponse>> ProdutosMaisVendidosAsync(
        FiltroPeriodoRequest filtro,
        int? limite = null,
        CancellationToken cancellationToken = default);
}
