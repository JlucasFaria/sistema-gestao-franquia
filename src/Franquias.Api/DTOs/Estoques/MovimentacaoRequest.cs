using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Estoques;

/// <summary>
/// Quantidade a dar entrada ou baixa no estoque de um item.
/// </summary>
public class MovimentacaoRequest
{
    /// <summary>Quantidade movimentada, sempre positiva. O sentido vem do endpoint chamado.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade movimentada deve ser maior que zero.")]
    public int Quantidade { get; set; }

    /// <summary>Justificativa da movimentação, como nota fiscal de compra ou motivo da perda.</summary>
    [MaxLength(250, ErrorMessage = "A observação deve ter no máximo {1} caracteres.")]
    public string? Observacao { get; set; }
}
