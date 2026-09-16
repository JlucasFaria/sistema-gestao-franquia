using Franquias.Api.DTOs.Royalties;

namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Royalties gerados no período, com o total da rede e a abertura por unidade. Uma cobrança
/// entra no relatório quando todo o seu período de apuração cabe no intervalo consultado.
/// </summary>
/// <param name="DataInicial">Primeiro dia considerado, ou nulo se o relatório é desde o início.</param>
/// <param name="DataFinal">Último dia considerado, ou nulo se o relatório vai até hoje.</param>
/// <param name="QuantidadeDeCobrancas">Cobranças geradas no período.</param>
/// <param name="TotalGerado">Soma do valor devido das cobranças.</param>
/// <param name="TotalPago">Soma do que já foi pago, incluindo multa e juros de atraso.</param>
/// <param name="TotalEmAberto">Soma do valor devido das cobranças ainda não quitadas.</param>
/// <param name="TotalEmAtraso">Soma do valor devido das cobranças vencidas e não quitadas.</param>
/// <param name="Unidades">Posição de cada unidade, da maior dívida em aberto para a menor.</param>
public sealed record RelatorioRoyaltiesResponse(
    DateOnly? DataInicial,
    DateOnly? DataFinal,
    int QuantidadeDeCobrancas,
    decimal TotalGerado,
    decimal TotalPago,
    decimal TotalEmAberto,
    decimal TotalEmAtraso,
    IReadOnlyList<ResumoRoyaltiesUnidadeResponse> Unidades);
