using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Perfis;

/// <summary>
/// Perfil de acesso a ser vinculado a um usuário.
/// </summary>
public class VincularPerfilRequest
{
    /// <summary>Identificador do perfil de acesso.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe um perfil válido.")]
    public int PerfilId { get; set; }
}
