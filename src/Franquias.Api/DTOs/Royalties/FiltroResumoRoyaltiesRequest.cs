using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Recorte de competências considerado no resumo de royalties. Uma cobrança entra no resumo
/// quando todo o seu período de apuração está dentro do recorte.
/// </summary>
public sealed class FiltroResumoRoyaltiesRequest : IValidatableObject
{
    /// <summary>Considera apenas cobranças cujo período começa nesta data ou depois.</summary>
    public DateOnly? CompetenciaInicio { get; init; }

    /// <summary>Considera apenas cobranças cujo período termina nesta data ou antes.</summary>
    public DateOnly? CompetenciaFim { get; init; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CompetenciaInicio is not null && CompetenciaFim is not null && CompetenciaFim < CompetenciaInicio)
        {
            yield return new ValidationResult(
                "O fim do recorte não pode ser anterior ao início.",
                [nameof(CompetenciaFim)]);
        }
    }
}
