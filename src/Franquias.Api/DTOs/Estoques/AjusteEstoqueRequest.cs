using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Estoques;

/// <summary>
/// Correção do saldo a partir de uma contagem física.
/// </summary>
public class AjusteEstoqueRequest
{
    /// <summary>
    /// Quantidade efetivamente encontrada na contagem. O sistema calcula sozinho a diferença
    /// em relação ao saldo atual: informa-se o que existe na prateleira, não a diferença.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade apurada não pode ser negativa.")]
    public int QuantidadeApurada { get; set; }

    /// <summary>Justificativa do ajuste, como o número do inventário.</summary>
    [Required(ErrorMessage = "A justificativa do ajuste é obrigatória.")]
    [MaxLength(250, ErrorMessage = "A observação deve ter no máximo {1} caracteres.")]
    public string Observacao { get; set; } = string.Empty;
}
