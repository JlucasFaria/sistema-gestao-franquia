using Franquias.Api.Common;

namespace Franquias.Api.Entities;

/// <summary>
/// Empresa detentora da marca, responsável pela rede de unidades franqueadas.
/// </summary>
public class Franqueadora : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Franqueadora()
    {
    }

    public Franqueadora(
        string razaoSocial,
        string nomeFantasia,
        string cnpj,
        string email,
        string telefone,
        Endereco endereco)
    {
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Cnpj = cnpj.SomenteDigitos();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        Endereco = endereco;
    }

    /// <summary>Razão social registrada da empresa.</summary>
    public string RazaoSocial { get; private set; } = string.Empty;

    /// <summary>Nome comercial da marca.</summary>
    public string NomeFantasia { get; private set; } = string.Empty;

    /// <summary>CNPJ gravado apenas com dígitos. Único em toda a base.</summary>
    public string Cnpj { get; private set; } = string.Empty;

    /// <summary>E-mail de contato da matriz.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Telefone de contato gravado apenas com dígitos.</summary>
    public string Telefone { get; private set; } = string.Empty;

    /// <summary>Endereço da matriz.</summary>
    public Endereco Endereco { get; private set; } = null!;

    /// <summary>
    /// Atualiza os dados cadastrais da franqueadora, registrando a data da alteração.
    /// O CNPJ não é alterável: identifica a empresa perante a Receita Federal.
    /// </summary>
    public void Atualizar(
        string razaoSocial,
        string nomeFantasia,
        string email,
        string telefone,
        Endereco endereco)
    {
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        Endereco = endereco;
        RegistrarAtualizacao();
    }
}
