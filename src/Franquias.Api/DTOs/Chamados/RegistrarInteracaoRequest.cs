using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Chamados;

/// <summary>
/// Mensagem acrescentada à linha do tempo de um chamado.
/// </summary>
public class RegistrarInteracaoRequest
{
    /// <summary>Conteúdo da mensagem.</summary>
    [Required(ErrorMessage = "Informe a mensagem da interação.")]
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "A mensagem deve ter de 2 a 2000 caracteres.")]
    public string Mensagem { get; set; } = string.Empty;
}
