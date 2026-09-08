using Franquias.Api.Common;

namespace Franquias.Api.Entities;

/// <summary>
/// Pessoa responsável pela operação de uma unidade franqueada, como o gerente ou o
/// sócio-administrador. Uma unidade pode ter mais de um responsável.
/// </summary>
public class Responsavel : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Responsavel()
    {
    }

    public Responsavel(
        string nome,
        string cpf,
        string cargo,
        string email,
        string telefone,
        int unidadeFranqueadaId)
    {
        Nome = nome;
        Cpf = cpf.SomenteDigitos();
        Cargo = cargo;
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        UnidadeFranqueadaId = unidadeFranqueadaId;
    }

    /// <summary>Nome completo do responsável.</summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>CPF gravado apenas com dígitos.</summary>
    public string Cpf { get; private set; } = string.Empty;

    /// <summary>Cargo exercido na unidade, como gerente ou sócio-administrador.</summary>
    public string Cargo { get; private set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Telefone de contato gravado apenas com dígitos.</summary>
    public string Telefone { get; private set; } = string.Empty;

    /// <summary>Chave estrangeira da unidade pela qual o responsável responde.</summary>
    public int UnidadeFranqueadaId { get; private set; }

    /// <summary>
    /// Atualiza cargo e dados de contato do responsável.
    /// </summary>
    public void Atualizar(string nome, string cargo, string email, string telefone)
    {
        Nome = nome;
        Cargo = cargo;
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        RegistrarAtualizacao();
    }
}
