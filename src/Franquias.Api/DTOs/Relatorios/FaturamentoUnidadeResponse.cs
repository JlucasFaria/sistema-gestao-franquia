namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Faturamento de uma unidade no período consultado.
/// </summary>
/// <param name="UnidadeFranqueadaId">Identificador da unidade.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="QuantidadeDeVendas">Vendas confirmadas no período.</param>
/// <param name="ValorTotal">Soma das vendas confirmadas.</param>
/// <param name="TicketMedio">Valor médio por venda confirmada.</param>
public sealed record FaturamentoUnidadeResponse(
    int UnidadeFranqueadaId,
    string Unidade,
    int QuantidadeDeVendas,
    decimal ValorTotal,
    decimal TicketMedio);
