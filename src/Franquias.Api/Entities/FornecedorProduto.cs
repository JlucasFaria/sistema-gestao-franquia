namespace Franquias.Api.Entities;

/// <summary>
/// Vínculo entre um fornecedor e um item do catálogo, com as condições comerciais
/// negociadas para esse par.
/// </summary>
public class FornecedorProduto : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected FornecedorProduto()
    {
    }

    public FornecedorProduto(
        int fornecedorId,
        int produtoServicoId,
        decimal precoFornecimento,
        int prazoEntregaEmDias)
    {
        FornecedorId = fornecedorId;
        ProdutoServicoId = produtoServicoId;
        PrecoFornecimento = precoFornecimento;
        PrazoEntregaEmDias = prazoEntregaEmDias;
    }

    /// <summary>Chave estrangeira do fornecedor.</summary>
    public int FornecedorId { get; private set; }

    /// <summary>Fornecedor do vínculo.</summary>
    public Fornecedor Fornecedor { get; private set; } = null!;

    /// <summary>Chave estrangeira do item do catálogo.</summary>
    public int ProdutoServicoId { get; private set; }

    /// <summary>Item do catálogo fornecido.</summary>
    public ProdutoServico ProdutoServico { get; private set; } = null!;

    /// <summary>Preço cobrado pelo fornecedor por unidade do item.</summary>
    public decimal PrecoFornecimento { get; private set; }

    /// <summary>Prazo de entrega acordado, em dias corridos.</summary>
    public int PrazoEntregaEmDias { get; private set; }

    /// <summary>
    /// Atualiza as condições comerciais negociadas com o fornecedor para este item.
    /// </summary>
    public void AtualizarCondicoes(decimal precoFornecimento, int prazoEntregaEmDias)
    {
        PrecoFornecimento = precoFornecimento;
        PrazoEntregaEmDias = prazoEntregaEmDias;
        RegistrarAtualizacao();
    }
}
