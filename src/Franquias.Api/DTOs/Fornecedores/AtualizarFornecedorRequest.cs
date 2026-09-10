using System.ComponentModel.DataAnnotations;
using Franquias.Api.DTOs.Enderecos;

namespace Franquias.Api.DTOs.Fornecedores;

/// <summary>
/// Dados cadastrais alteráveis de um fornecedor. O CNPJ não consta: identifica a empresa.
/// </summary>
public class AtualizarFornecedorRequest
{
    /// <summary>Razão social registrada.</summary>
    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [MaxLength(180, ErrorMessage = "A razão social deve ter no máximo {1} caracteres.")]
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome comercial do fornecedor.</summary>
    [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome fantasia deve ter no máximo {1} caracteres.")]
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(180, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Telefone com DDD, com ou sem máscara.</summary>
    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [RegularExpression(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$",
        ErrorMessage = "Informe um telefone com DDD, como (11) 3333-4444.")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>Endereço do fornecedor.</summary>
    [Required(ErrorMessage = "O endereço é obrigatório.")]
    public EnderecoDto Endereco { get; set; } = new();
}
