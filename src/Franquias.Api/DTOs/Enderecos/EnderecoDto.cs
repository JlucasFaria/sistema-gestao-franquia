using System.ComponentModel.DataAnnotations;
using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Enderecos;

/// <summary>
/// Endereço trafegado pela API, usado tanto na entrada quanto na saída.
/// </summary>
public class EnderecoDto
{
    /// <summary>Nome da rua, avenida ou praça.</summary>
    [Required(ErrorMessage = "O logradouro é obrigatório.")]
    [MaxLength(180, ErrorMessage = "O logradouro deve ter no máximo {1} caracteres.")]
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>Número do imóvel.</summary>
    [Required(ErrorMessage = "O número é obrigatório.")]
    [MaxLength(10, ErrorMessage = "O número deve ter no máximo {1} caracteres.")]
    public string Numero { get; set; } = string.Empty;

    /// <summary>Complemento, como sala, andar ou bloco.</summary>
    [MaxLength(60, ErrorMessage = "O complemento deve ter no máximo {1} caracteres.")]
    public string? Complemento { get; set; }

    /// <summary>Bairro.</summary>
    [Required(ErrorMessage = "O bairro é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O bairro deve ter no máximo {1} caracteres.")]
    public string Bairro { get; set; } = string.Empty;

    /// <summary>Município.</summary>
    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [MaxLength(100, ErrorMessage = "A cidade deve ter no máximo {1} caracteres.")]
    public string Cidade { get; set; } = string.Empty;

    /// <summary>Sigla da unidade federativa.</summary>
    [Required(ErrorMessage = "A UF é obrigatória.")]
    [RegularExpression("^[A-Za-z]{2}$", ErrorMessage = "A UF deve ter duas letras.")]
    public string Uf { get; set; } = string.Empty;

    /// <summary>CEP, com ou sem hífen.</summary>
    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Informe um CEP válido, como 01310-100.")]
    public string Cep { get; set; } = string.Empty;

    /// <summary>Converte no objeto de valor do domínio, que normaliza UF e CEP.</summary>
    public Endereco ParaEntidade() =>
        new(Logradouro, Numero, Complemento, Bairro, Cidade, Uf, Cep);

    /// <summary>Projeta o objeto de valor no DTO de saída.</summary>
    public static EnderecoDto De(Endereco endereco) => new()
    {
        Logradouro = endereco.Logradouro,
        Numero = endereco.Numero,
        Complemento = endereco.Complemento,
        Bairro = endereco.Bairro,
        Cidade = endereco.Cidade,
        Uf = endereco.Uf,
        Cep = endereco.Cep
    };
}
