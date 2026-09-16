namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Posição de uma unidade no ranking de faturamento da rede.
/// </summary>
/// <param name="Posicao">Colocação, a partir de 1.</param>
/// <param name="UnidadeFranqueadaId">Identificador da unidade.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="QuantidadeDeVendas">Vendas confirmadas no período.</param>
/// <param name="ValorTotal">Faturamento da unidade no período.</param>
/// <param name="ParticipacaoPercentual">Quanto a unidade representa do faturamento da rede.</param>
public sealed record RankingUnidadeResponse(
    int Posicao,
    int UnidadeFranqueadaId,
    string Unidade,
    int QuantidadeDeVendas,
    decimal ValorTotal,
    decimal ParticipacaoPercentual);
