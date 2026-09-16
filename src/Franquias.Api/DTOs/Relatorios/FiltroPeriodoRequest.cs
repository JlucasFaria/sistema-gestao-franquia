using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Intervalo de datas dos relatórios. As duas pontas são opcionais e inclusivas: sem nenhuma
/// delas, o relatório cobre toda a história da rede.
/// </summary>
public class FiltroPeriodoRequest : IValidatableObject
{
    /// <summary>Primeiro dia considerado, inclusive.</summary>
    public DateOnly? DataInicial { get; set; }

    /// <summary>Último dia considerado, inclusive.</summary>
    public DateOnly? DataFinal { get; set; }

    /// <summary>
    /// Recusa intervalo invertido. Sem essa checagem, o relatório voltaria vazio e quem
    /// consultou não saberia que errou as datas.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataInicial is not null && DataFinal is not null && DataInicial > DataFinal)
        {
            yield return new ValidationResult(
                "A data inicial não pode ser posterior à data final.",
                [nameof(DataInicial), nameof(DataFinal)]);
        }
    }
}
