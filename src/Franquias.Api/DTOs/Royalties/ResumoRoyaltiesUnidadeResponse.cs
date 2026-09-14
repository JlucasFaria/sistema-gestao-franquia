namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Posição financeira de royalties de uma unidade: quanto foi cobrado, quanto foi pago e
/// quanto ainda está em aberto.
/// </summary>
/// <param name="UnidadeFranqueadaId">Identificador da unidade.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="QuantidadeDeCobrancas">Quantidade de cobranças consideradas.</param>
/// <param name="TotalDevido">Soma do valor devido de todas as cobranças, pagas ou não.</param>
/// <param name="TotalPago">
/// Soma dos valores efetivamente pagos. Pode superar o devido das cobranças quitadas quando
/// o pagamento em atraso incluiu multa e juros.
/// </param>
/// <param name="TotalEmAberto">Soma do valor devido das cobranças ainda não quitadas.</param>
/// <param name="QuantidadeEmAtraso">Quantidade de cobranças vencidas e não quitadas.</param>
/// <param name="TotalEmAtraso">Soma do valor devido das cobranças atrasadas.</param>
public sealed record ResumoRoyaltiesUnidadeResponse(
    int UnidadeFranqueadaId,
    string Unidade,
    int QuantidadeDeCobrancas,
    decimal TotalDevido,
    decimal TotalPago,
    decimal TotalEmAberto,
    int QuantidadeEmAtraso,
    decimal TotalEmAtraso)
{
    /// <summary>
    /// Resumo zerado, para a unidade que ainda não tem cobranças no recorte consultado.
    /// </summary>
    public static ResumoRoyaltiesUnidadeResponse Vazio(int unidadeFranqueadaId, string unidade) =>
        new(unidadeFranqueadaId, unidade, 0, decimal.Zero, decimal.Zero, decimal.Zero, 0, decimal.Zero);
}
