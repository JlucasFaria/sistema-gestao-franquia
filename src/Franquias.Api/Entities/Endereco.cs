using Franquias.Api.Common;

namespace Franquias.Api.Entities;

/// <summary>
/// Endereço de uma franqueadora ou unidade franqueada. É um objeto de valor: não tem
/// identidade própria e é persistido nas colunas da entidade que o contém.
/// </summary>
public class Endereco
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Endereco()
    {
    }

    public Endereco(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string uf,
        string cep)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cidade = cidade;
        Uf = uf.Trim().ToUpperInvariant();
        Cep = cep.SomenteDigitos();
    }

    /// <summary>Nome da rua, avenida ou praça.</summary>
    public string Logradouro { get; private set; } = string.Empty;

    /// <summary>Número do imóvel.</summary>
    public string Numero { get; private set; } = string.Empty;

    /// <summary>Complemento, como sala, andar ou bloco.</summary>
    public string? Complemento { get; private set; }

    /// <summary>Bairro.</summary>
    public string Bairro { get; private set; } = string.Empty;

    /// <summary>Município.</summary>
    public string Cidade { get; private set; } = string.Empty;

    /// <summary>Sigla da unidade federativa, com dois caracteres.</summary>
    public string Uf { get; private set; } = string.Empty;

    /// <summary>CEP gravado apenas com dígitos.</summary>
    public string Cep { get; private set; } = string.Empty;
}
