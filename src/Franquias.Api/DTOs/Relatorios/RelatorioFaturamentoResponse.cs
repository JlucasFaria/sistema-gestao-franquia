namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Faturamento da rede no período, aberto por unidade. Considera apenas vendas confirmadas:
/// pendentes ainda podem não se concretizar e canceladas não representam receita.
/// </summary>
/// <param name="DataInicial">Primeiro dia considerado, ou nulo se o relatório é desde o início.</param>
/// <param name="DataFinal">Último dia considerado, ou nulo se o relatório vai até hoje.</param>
/// <param name="QuantidadeDeVendas">Vendas confirmadas no período, somando todas as unidades.</param>
/// <param name="TotalGeral">Faturamento da rede no período.</param>
/// <param name="TicketMedio">Valor médio por venda confirmada da rede.</param>
/// <param name="Unidades">Faturamento de cada unidade, da maior para a menor.</param>
public sealed record RelatorioFaturamentoResponse(
    DateOnly? DataInicial,
    DateOnly? DataFinal,
    int QuantidadeDeVendas,
    decimal TotalGeral,
    decimal TicketMedio,
    IReadOnlyList<FaturamentoUnidadeResponse> Unidades);
