using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IRelatorioService"/>.
/// </summary>
public sealed class RelatorioService(IRelatorioRepositorio relatorios) : IRelatorioService
{
    /// <inheritdoc />
    public async Task<RelatorioFaturamentoResponse> FaturamentoPorUnidadeAsync(
        FiltroPeriodoRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var unidades = await relatorios.FaturamentoPorUnidadeAsync(
            filtro.DataInicial,
            filtro.DataFinal,
            cancellationToken);

        // A ordenação acontece aqui porque o SQLite não ordena por colunas decimais.
        var ordenadas = unidades
            .OrderByDescending(unidade => unidade.ValorTotal)
            .ThenBy(unidade => unidade.Unidade)
            .ToList();

        var quantidade = ordenadas.Sum(unidade => unidade.QuantidadeDeVendas);
        var total = ordenadas.Sum(unidade => unidade.ValorTotal);

        return new RelatorioFaturamentoResponse(
            filtro.DataInicial,
            filtro.DataFinal,
            quantidade,
            total,
            quantidade == 0 ? decimal.Zero : Math.Round(total / quantidade, 2, MidpointRounding.AwayFromZero),
            ordenadas);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RankingUnidadeResponse>> RankingPorFaturamentoAsync(
        FiltroPeriodoRequest filtro,
        int? limite = null,
        CancellationToken cancellationToken = default)
    {
        var faturamento = await FaturamentoPorUnidadeAsync(filtro, cancellationToken);

        var ranking = faturamento.Unidades
            .Select((unidade, indice) => new RankingUnidadeResponse(
                indice + 1,
                unidade.UnidadeFranqueadaId,
                unidade.Unidade,
                unidade.QuantidadeDeVendas,
                unidade.ValorTotal,
                CalcularParticipacao(unidade.ValorTotal, faturamento.TotalGeral)));

        if (limite is not null)
        {
            ranking = ranking.Take(limite.Value);
        }

        return [.. ranking];
    }

    /// <summary>
    /// Participação da unidade no faturamento da rede, em pontos percentuais com duas casas.
    /// Rede sem faturamento no período resulta em zero, e não em divisão por zero.
    /// </summary>
    private static decimal CalcularParticipacao(decimal valor, decimal total) =>
        total == decimal.Zero
            ? decimal.Zero
            : Math.Round(valor * 100m / total, 2, MidpointRounding.AwayFromZero);
}
