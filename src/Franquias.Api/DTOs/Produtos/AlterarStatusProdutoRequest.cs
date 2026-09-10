using System.ComponentModel.DataAnnotations;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Produtos;

/// <summary>
/// Nova situação de um item no catálogo.
/// </summary>
public class AlterarStatusProdutoRequest
{
    /// <summary>Situação a aplicar: Ativo, Inativo ou Descontinuado.</summary>
    [Required(ErrorMessage = "O status é obrigatório.")]
    [EnumDataType(typeof(StatusProduto), ErrorMessage = "Status inválido.")]
    public StatusProduto? Status { get; set; }
}
