using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Cobrança de royalty devolvida pela API.
/// </summary>
/// <param name="Id">Identificador da cobrança.</param>
/// <param name="UnidadeFranqueadaId">Unidade cobrada.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="PeriodoInicio">Primeiro dia do período de apuração.</param>
/// <param name="PeriodoFim">Último dia do período de apuração.</param>
/// <param name="FaturamentoBase">Faturamento confirmado no período, base do cálculo.</param>
/// <param name="PercentualAplicado">Percentual vigente na unidade quando a cobrança foi gerada.</param>
/// <param name="ValorDevido">Faturamento vezes percentual, arredondado a centavos.</param>
/// <param name="DataVencimento">Data limite para pagamento.</param>
/// <param name="Situacao">Pendente, Pago ou Atrasado.</param>
/// <param name="DataPagamento">Data da quitação, quando houver.</param>
/// <param name="ValorPago">Valor efetivamente pago, quando houver.</param>
/// <param name="EmAberto">Indica que a cobrança ainda não foi quitada.</param>
/// <param name="DataCriacao">Momento da geração, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última mudança de situação, em UTC.</param>
public sealed record RoyaltyResponse(
    int Id,
    int UnidadeFranqueadaId,
    string Unidade,
    DateOnly PeriodoInicio,
    DateOnly PeriodoFim,
    decimal FaturamentoBase,
    decimal PercentualAplicado,
    decimal ValorDevido,
    DateOnly DataVencimento,
    SituacaoPagamento Situacao,
    DateOnly? DataPagamento,
    decimal? ValorPago,
    bool EmAberto,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>Projeta a entidade no DTO de saída. Espera a unidade carregada.</summary>
    public static RoyaltyResponse De(Royalty royalty) => new(
        royalty.Id,
        royalty.UnidadeFranqueadaId,
        royalty.UnidadeFranqueada?.NomeFantasia ?? string.Empty,
        royalty.PeriodoInicio,
        royalty.PeriodoFim,
        royalty.FaturamentoBase,
        royalty.PercentualAplicado,
        royalty.ValorDevido,
        royalty.DataVencimento,
        royalty.Situacao,
        royalty.DataPagamento,
        royalty.ValorPago,
        royalty.EstaEmAberto(),
        royalty.DataCriacao,
        royalty.DataAtualizacao);
}
