using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Usuarios;

/// <summary>
/// Dados para cadastro de um usuário.
/// </summary>
public class CriarUsuarioRequest
{
    /// <summary>Nome completo do usuário.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>E-mail de login. Precisa ser único na base.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(180, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Senha de acesso, armazenada apenas como hash.</summary>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(72, MinimumLength = 8,
        ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.")]
    public string Senha { get; set; } = string.Empty;

    /// <summary>Identificador do perfil de acesso concedido.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe um perfil válido.")]
    public int PerfilId { get; set; }
}
