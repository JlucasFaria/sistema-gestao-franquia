using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Categorias;

/// <summary>
/// Dados de uma categoria, usados tanto no cadastro quanto na alteração.
/// </summary>
public class CategoriaRequest
{
    /// <summary>Nome da categoria. Precisa ser único, sem diferenciar maiúsculas de minúsculas.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>Descrição do que a categoria agrupa.</summary>
    [MaxLength(250, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
    public string? Descricao { get; set; }
}
