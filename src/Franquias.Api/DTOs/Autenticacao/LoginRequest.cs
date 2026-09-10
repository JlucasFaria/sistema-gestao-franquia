using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Autenticacao;

/// <summary>
/// Credenciais enviadas para autenticação.
/// </summary>
public class LoginRequest
{
    /// <summary>E-mail cadastrado do usuário.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(180, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Senha em texto puro, conferida contra o hash armazenado.</summary>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}
