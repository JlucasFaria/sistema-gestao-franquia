namespace Franquias.Api.Entities;

/// <summary>
/// Item de uma venda. Guarda o preço praticado no momento da venda, e não o preço de
/// tabela do catálogo, para que reajustes futuros não alterem o faturamento já apurado.
/// </summary>
public class ItemVenda : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected ItemVenda()
    {
    }

    internal ItemVenda(int produtoServicoId, int quantidade, decimal precoUnitario)
    {
        ProdutoServicoId = produtoServicoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        Subtotal = quantidade * precoUnitario;
    }

    /// <summary>Chave estrangeira da venda a que o item pertence.</summary>
    public int VendaId { get; private set; }

    /// <summary>Venda a que o item pertence.</summary>
    public Venda Venda { get; private set; } = null!;

    /// <summary>Chave estrangeira do item do catálogo vendido.</summary>
    public int ProdutoServicoId { get; private set; }

    /// <summary>Item do catálogo vendido.</summary>
    public ProdutoServico ProdutoServico { get; private set; } = null!;

    /// <summary>Quantidade vendida.</summary>
    public int Quantidade { get; private set; }

    /// <summary>Preço unitário praticado na venda.</summary>
    public decimal PrecoUnitario { get; private set; }

    /// <summary>Resultado de <see cref="Quantidade"/> vezes <see cref="PrecoUnitario"/>.</summary>
    public decimal Subtotal { get; private set; }
}
