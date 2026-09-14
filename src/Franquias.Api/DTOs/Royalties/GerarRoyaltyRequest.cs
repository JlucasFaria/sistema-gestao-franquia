using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Pedido de geração da cobrança de royalty de uma unidade em um período. O valor não é
/// informado: é calculado sobre o faturamento confirmado no período.
/// </summary>
public class GerarRoyaltyRequest : IValidatableObject
{
    /// <summary>Unidade a ser cobrada.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma unidade válida.")]
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Primeiro dia do período de apuração.</summary>
    [Required(ErrorMessage = "O início do período é obrigatório.")]
    public DateOnly? PeriodoInicio { get; set; }

    /// <summary>Último dia do período de apuração, inclusive.</summary>
    [Required(ErrorMessage = "O fim do período é obrigatório.")]
    public DateOnly? PeriodoFim { get; set; }

    /// <summary>Data limite para pagamento. Precisa ser posterior ao fim do período.</summary>
    [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
    public DateOnly? DataVencimento { get; set; }

    /// <summary>
    /// Confere a coerência entre as datas: período não invertido e vencimento depois do
    /// fechamento, já que a cobrança só existe quando o faturamento do período está fechado.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PeriodoInicio is not null && PeriodoFim is not null && PeriodoFim < PeriodoInicio)
        {
            yield return new ValidationResult(
                "O fim do período não pode ser anterior ao início.",
                [nameof(PeriodoInicio), nameof(PeriodoFim)]);
        }

        if (PeriodoFim is not null && DataVencimento is not null && DataVencimento <= PeriodoFim)
        {
            yield return new ValidationResult(
                "O vencimento precisa ser posterior ao fim do período de apuração.",
                [nameof(DataVencimento)]);
        }
    }
}
