using Franquias.Api.Common;

namespace Franquias.Api.Entities;

/// <summary>
/// Pessoa que firmou o contrato de franquia com a franqueadora. Um franqueado pode
/// responder por mais de uma unidade da rede.
/// </summary>
public class Franqueado : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Franqueado()
    {
    }

    public Franqueado(string nome, string cpf, string email, string telefone, DateOnly dataAdesao)
    {
        Nome = nome;
        Cpf = cpf.SomenteDigitos();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        DataAdesao = dataAdesao;
    }

    /// <summary>Nome completo do franqueado.</summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>CPF gravado apenas com dígitos. Único em toda a base.</summary>
    public string Cpf { get; private set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Telefone de contato gravado apenas com dígitos.</summary>
    public string Telefone { get; private set; } = string.Empty;

    /// <summary>Data em que o franqueado aderiu à rede.</summary>
    public DateOnly DataAdesao { get; private set; }

    /// <summary>
    /// Atualiza os dados de contato do franqueado. O CPF é imutável: identifica a pessoa
    /// que assinou o contrato de franquia.
    /// </summary>
    public void Atualizar(string nome, string email, string telefone)
    {
        Nome = nome;
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        RegistrarAtualizacao();
    }
}
