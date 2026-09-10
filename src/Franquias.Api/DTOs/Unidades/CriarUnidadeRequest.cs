using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;
using Franquias.Api.DTOs.Enderecos;

namespace Franquias.Api.DTOs.Unidades;

/// <summary>
/// Dados para cadastro de uma unidade franqueada. A unidade nasce em implantação.
/// </summary>
public class CriarUnidadeRequest
{
    /// <summary>Franqueadora dona da marca.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma franqueadora válida.")]
    public int FranqueadoraId { get; set; }

    /// <summary>Franqueado que vai operar a unidade.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe um franqueado válido.")]
    public int FranqueadoId { get; set; }

    /// <summary>Razão social da unidade.</summary>
    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [MaxLength(180, ErrorMessage = "A razão social deve ter no máximo {1} caracteres.")]
    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Nome pelo qual a unidade é conhecida.</summary>
    [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome fantasia deve ter no máximo {1} caracteres.")]
    public string NomeFantasia { get; set; } = string.Empty;

    /// <summary>CNPJ, com ou sem máscara. Precisa ser único na base.</summary>
    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [Cnpj]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>E-mail de contato da unidade.</summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(180, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Telefone com DDD, com ou sem máscara.</summary>
    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [RegularExpression(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$",
        ErrorMessage = "Informe um telefone com DDD, como (11) 3333-4444.")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>Endereço da unidade.</summary>
    [Required(ErrorMessage = "O endereço é obrigatório.")]
    public EnderecoDto Endereco { get; set; } = new();

    /// <summary>Data de início da operação prevista em contrato.</summary>
    [Required(ErrorMessage = "A data de início é obrigatória.")]
    public DateOnly? DataInicio { get; set; }

    /// <summary>Percentual de royalty sobre o faturamento, entre 0 e 100.</summary>
    [Range(typeof(decimal), "0", "100", ErrorMessage = "O percentual de royalty deve estar entre {1} e {2}.")]
    public decimal PercentualRoyalty { get; set; }
}
