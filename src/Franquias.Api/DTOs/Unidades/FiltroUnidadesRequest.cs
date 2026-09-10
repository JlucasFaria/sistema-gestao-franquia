using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Unidades;

/// <summary>
/// Filtros estruturados da listagem de unidades. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroUnidadesRequest
{
    /// <summary>Restringe a uma situação contratual.</summary>
    public SituacaoUnidade? Situacao { get; set; }

    /// <summary>Restringe às unidades de uma franqueadora.</summary>
    public int? FranqueadoraId { get; set; }

    /// <summary>Restringe às unidades de um franqueado.</summary>
    public int? FranqueadoId { get; set; }
}
