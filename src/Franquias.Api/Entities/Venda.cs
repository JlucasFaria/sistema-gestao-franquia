using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Venda registrada por uma unidade franqueada. O valor total é sempre derivado dos itens,
/// nunca informado de fora.
/// </summary>
public class Venda : EntidadeBase
{
    private readonly List<ItemVenda> _itens = [];

    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Venda()
    {
    }

    public Venda(int unidadeFranqueadaId)
    {
        UnidadeFranqueadaId = unidadeFranqueadaId;
        DataVenda = DateTime.UtcNow;
        Status = StatusVenda.Pendente;
        ValorTotal = decimal.Zero;
    }

    /// <summary>Chave estrangeira da unidade que realizou a venda.</summary>
    public int UnidadeFranqueadaId { get; private set; }

    /// <summary>Unidade que realizou a venda.</summary>
    public UnidadeFranqueada UnidadeFranqueada { get; private set; } = null!;

    /// <summary>Momento da venda, em UTC. É a data usada nos relatórios por período.</summary>
    public DateTime DataVenda { get; private set; }

    /// <summary>Estágio da venda.</summary>
    public StatusVenda Status { get; private set; }

    /// <summary>Soma dos subtotais dos itens.</summary>
    public decimal ValorTotal { get; private set; }

    /// <summary>Itens que compõem a venda.</summary>
    public IReadOnlyCollection<ItemVenda> Itens => _itens;

    /// <summary>
    /// Acrescenta um item à venda e recalcula o total.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se a venda não estiver mais em aberto.</exception>
    public ItemVenda AdicionarItem(int produtoServicoId, int quantidade, decimal precoUnitario)
    {
        ExigirVendaEmAberto();

        if (quantidade <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidade),
                "A quantidade vendida deve ser maior que zero.");
        }

        if (precoUnitario < decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(precoUnitario),
                "O preço unitário não pode ser negativo.");
        }

        var item = new ItemVenda(produtoServicoId, quantidade, precoUnitario);

        _itens.Add(item);
        RecalcularTotal();

        return item;
    }

    /// <summary>
    /// Remove um item da venda e recalcula o total.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se a venda não estiver mais em aberto.</exception>
    public void RemoverItem(ItemVenda item)
    {
        ExigirVendaEmAberto();

        _itens.Remove(item);
        RecalcularTotal();
    }

    /// <summary>
    /// Conclui a venda. A partir daqui os itens não podem mais ser alterados e o estoque
    /// já deve ter sido baixado pelo serviço de vendas, dentro da mesma transação.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se a venda não estiver em aberto ou não tiver itens.</exception>
    public void Confirmar()
    {
        ExigirVendaEmAberto();

        if (_itens.Count == 0)
        {
            throw new InvalidOperationException("Uma venda precisa de pelo menos um item para ser confirmada.");
        }

        Status = StatusVenda.Confirmada;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Cancela a venda. O estorno do estoque é responsabilidade do serviço de vendas.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se a venda já estiver cancelada.</exception>
    public void Cancelar()
    {
        if (Status == StatusVenda.Cancelada)
        {
            throw new InvalidOperationException("A venda já está cancelada.");
        }

        Status = StatusVenda.Cancelada;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Indica se a venda compõe o faturamento apurado para royalties e relatórios.
    /// </summary>
    public bool ComputaFaturamento() => Status == StatusVenda.Confirmada;

    private void RecalcularTotal()
    {
        ValorTotal = _itens.Sum(item => item.Subtotal);
        RegistrarAtualizacao();
    }

    private void ExigirVendaEmAberto()
    {
        if (Status != StatusVenda.Pendente)
        {
            throw new InvalidOperationException(
                $"A venda não pode ser alterada porque está com status {Status}.");
        }
    }
}
