using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Usuarios;

/// <summary>
/// Dados alteráveis de um usuário. A senha não é alterada por aqui.
/// </summary>
public class AtualizarUsuarioRequest
{
    /// <summary>Nome completo do usuário.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail de login. Precisa continuar único na base.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(180, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;
}
