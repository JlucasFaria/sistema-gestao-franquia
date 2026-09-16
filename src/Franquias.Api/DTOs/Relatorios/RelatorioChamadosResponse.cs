namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Indicador de chamados por estágio de atendimento. Todos os estágios aparecem, inclusive
/// os que estão zerados, para que o painel não mude de formato conforme o movimento do dia.
/// </summary>
/// <param name="DataInicial">Primeiro dia de abertura considerado, ou nulo se desde o início.</param>
/// <param name="DataFinal">Último dia de abertura considerado, ou nulo se até hoje.</param>
/// <param name="Total">Chamados abertos no período.</param>
/// <param name="TotalEmAberto">Chamados que ainda não foram encerrados.</param>
/// <param name="PorStatus">Quantidade em cada estágio de atendimento.</param>
public sealed record RelatorioChamadosResponse(
    DateOnly? DataInicial,
    DateOnly? DataFinal,
    int Total,
    int TotalEmAberto,
    IReadOnlyList<ContagemPorStatusResponse> PorStatus);
