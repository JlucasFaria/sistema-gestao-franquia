namespace Franquias.Api.DTOs.Estoques;

/// <summary>
/// Filtros estruturados da consulta de saldos. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroEstoquesRequest
{
    /// <summary>Restringe aos saldos de uma unidade franqueada.</summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>Restringe aos saldos de um item do catálogo, em todas as unidades.</summary>
    public int? ProdutoServicoId { get; set; }
}
