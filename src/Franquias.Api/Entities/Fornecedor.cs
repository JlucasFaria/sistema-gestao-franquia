using Franquias.Api.Common;

namespace Franquias.Api.Entities;

/// <summary>
/// Empresa homologada pela franqueadora para fornecer produtos e serviços às unidades.
/// </summary>
public class Fornecedor : EntidadeBase
{
    private readonly List<FornecedorProduto> _produtos = [];

    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Fornecedor()
    {
    }

    public Fornecedor(
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

    /// <summary>Razão social registrada do fornecedor.</summary>
    public string RazaoSocial { get; private set; } = string.Empty;

    /// <summary>Nome comercial do fornecedor.</summary>
    public string NomeFantasia { get; private set; } = string.Empty;

    /// <summary>CNPJ gravado apenas com dígitos. Único em toda a base.</summary>
    public string Cnpj { get; private set; } = string.Empty;

    /// <summary>E-mail de contato.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Telefone de contato gravado apenas com dígitos.</summary>
    public string Telefone { get; private set; } = string.Empty;

    /// <summary>Endereço do fornecedor.</summary>
    public Endereco Endereco { get; private set; } = null!;

    /// <summary>Itens do catálogo que este fornecedor é homologado a fornecer.</summary>
    public IReadOnlyCollection<FornecedorProduto> Produtos => _produtos;

    /// <summary>
    /// Atualiza os dados cadastrais do fornecedor. O CNPJ é imutável.
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

    /// <summary>
    /// Homologa o fornecedor para um item do catálogo.
    /// </summary>
    public void AssociarProduto(FornecedorProduto vinculo)
    {
        _produtos.Add(vinculo);
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Remove a homologação do fornecedor para um item do catálogo.
    /// </summary>
    public void DesassociarProduto(FornecedorProduto vinculo)
    {
        _produtos.Remove(vinculo);
        RegistrarAtualizacao();
    }
}
