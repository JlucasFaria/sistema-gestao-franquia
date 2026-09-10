using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;

namespace Franquias.Api.DTOs.Franqueados;

/// <summary>
/// Dados para cadastro de um franqueado.
/// </summary>
public class CriarFranqueadoRequest
{
    /// <summary>Nome completo.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>CPF, com ou sem máscara. Precisa ser único na base.</summary>
    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [Cpf]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(180, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Telefone com DDD, com ou sem máscara.</summary>
    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [RegularExpression(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$",
        ErrorMessage = "Informe um telefone com DDD, como (11) 99999-8888.")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>Data de adesão à rede.</summary>
    [Required(ErrorMessage = "A data de adesão é obrigatória.")]
    public DateOnly? DataAdesao { get; set; }
}
