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
}
