using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IRelatorioService"/>.
/// </summary>
public sealed class RelatorioService(
    IRelatorioRepositorio relatorios,
    IRoyaltyService royalties) : IRelatorioService
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

    /// <inheritdoc />
    public async Task<RelatorioRoyaltiesResponse> RoyaltiesGeradosAsync(
        FiltroPeriodoRequest filtro,
        CancellationToken cancellationToken = default)
    {
        // O resumo do serviço de royalties já marca como atrasadas as cobranças vencidas,
        // então o total em atraso do relatório reflete a situação de hoje.
        var recorte = new FiltroResumoRoyaltiesRequest
        {
            CompetenciaInicio = filtro.DataInicial,
            CompetenciaFim = filtro.DataFinal
        };

        var unidades = await royalties.ResumirPorUnidadeAsync(recorte, cancellationToken);

        return new RelatorioRoyaltiesResponse(
            filtro.DataInicial,
            filtro.DataFinal,
            unidades.Sum(unidade => unidade.QuantidadeDeCobrancas),
            unidades.Sum(unidade => unidade.TotalDevido),
            unidades.Sum(unidade => unidade.TotalPago),
            unidades.Sum(unidade => unidade.TotalEmAberto),
            unidades.Sum(unidade => unidade.TotalEmAtraso),
            unidades);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProdutoMaisVendidoResponse>> ProdutosMaisVendidosAsync(
        FiltroPeriodoRequest filtro,
        int? limite = null,
        CancellationToken cancellationToken = default)
    {
        var itens = await relatorios.ProdutosMaisVendidosAsync(
            filtro.DataInicial,
            filtro.DataFinal,
            cancellationToken);

        // Mais vendido é o que saiu em maior quantidade. A receita desempata, porque entre
        // dois itens com a mesma saída o que rendeu mais pesa mais para a rede.
        var ordenados = itens
            .OrderByDescending(item => item.QuantidadeVendida)
            .ThenByDescending(item => item.ValorTotal)
            .ThenBy(item => item.Produto)
            .Select((item, indice) => item with { Posicao = indice + 1 });

        if (limite is not null)
        {
            ordenados = ordenados.Take(limite.Value);
        }

        return [.. ordenados];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EstoqueCriticoResponse>> EstoqueCriticoAsync(
        int? unidadeFranqueadaId = null,
        CancellationToken cancellationToken = default) =>
        await relatorios.EstoqueCriticoAsync(unidadeFranqueadaId, cancellationToken);
}
