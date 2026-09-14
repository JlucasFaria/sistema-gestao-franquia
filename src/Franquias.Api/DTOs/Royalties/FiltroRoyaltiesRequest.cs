using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Filtros da consulta de cobranças de royalty. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroRoyaltiesRequest
{
    /// <summary>Restringe às cobranças de uma unidade.</summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>Restringe a uma situação de pagamento: Pendente, Pago ou Atrasado.</summary>
    public SituacaoPagamento? Situacao { get; set; }
}
