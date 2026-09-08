using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Saldo de um item do catálogo em uma unidade franqueada. O estoque é controlado por
/// unidade: o mesmo produto tem saldos independentes em cada loja da rede.
/// </summary>
public class Estoque : EntidadeBase
{
    private readonly List<MovimentacaoEstoque> _movimentacoes = [];

    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Estoque()
    {
    }

    public Estoque(int unidadeFranqueadaId, int produtoServicoId, int quantidadeMinima)
    {
        UnidadeFranqueadaId = unidadeFranqueadaId;
        ProdutoServicoId = produtoServicoId;
        QuantidadeMinima = quantidadeMinima;
        Quantidade = 0;
    }

    /// <summary>Chave estrangeira da unidade dona do saldo.</summary>
    public int UnidadeFranqueadaId { get; private set; }

    /// <summary>Unidade dona do saldo.</summary>
    public UnidadeFranqueada UnidadeFranqueada { get; private set; } = null!;

    /// <summary>Chave estrangeira do item do catálogo.</summary>
    public int ProdutoServicoId { get; private set; }

    /// <summary>Item do catálogo controlado.</summary>
    public ProdutoServico ProdutoServico { get; private set; } = null!;

    /// <summary>Saldo disponível. Nunca é negativo.</summary>
    public int Quantidade { get; private set; }

    /// <summary>Saldo abaixo do qual a unidade deve repor o item.</summary>
    public int QuantidadeMinima { get; private set; }

    /// <summary>Histórico de movimentações deste saldo.</summary>
    public IReadOnlyCollection<MovimentacaoEstoque> Movimentacoes => _movimentacoes;

    /// <summary>
    /// Indica se o saldo comporta a baixa da quantidade informada.
    /// </summary>
    public bool PossuiSaldoPara(int quantidade) => Quantidade >= quantidade;

    /// <summary>
    /// Indica se o saldo atingiu o ponto de reposição.
    /// </summary>
    public bool EstaAbaixoDoMinimo() => Quantidade < QuantidadeMinima;

    /// <summary>
    /// Acresce quantidade ao saldo e devolve a movimentação correspondente.
    /// </summary>
    public MovimentacaoEstoque RegistrarEntrada(int quantidade, string? observacao = null)
    {
        ExigirQuantidadePositiva(quantidade);
        return Movimentar(TipoMovimentacao.Entrada, quantidade, Quantidade + quantidade, observacao);
    }

    /// <summary>
    /// Baixa quantidade do saldo e devolve a movimentação correspondente.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se o saldo for insuficiente.</exception>
    public MovimentacaoEstoque RegistrarSaida(int quantidade, string? observacao = null)
    {
        ExigirQuantidadePositiva(quantidade);

        if (!PossuiSaldoPara(quantidade))
        {
            throw new InvalidOperationException(
                $"Saldo insuficiente: disponível {Quantidade}, solicitado {quantidade}.");
        }

        return Movimentar(TipoMovimentacao.Saida, quantidade, Quantidade - quantidade, observacao);
    }

    /// <summary>
    /// Corrige o saldo para a quantidade apurada em inventário e devolve a movimentação
    /// correspondente.
    /// </summary>
    public MovimentacaoEstoque Ajustar(int quantidadeApurada, string? observacao = null)
    {
        if (quantidadeApurada < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidadeApurada),
                "O saldo apurado não pode ser negativo.");
        }

        var diferenca = Math.Abs(quantidadeApurada - Quantidade);
        return Movimentar(TipoMovimentacao.Ajuste, diferenca, quantidadeApurada, observacao);
    }

    /// <summary>
    /// Define o ponto de reposição do item nesta unidade.
    /// </summary>
    public void DefinirQuantidadeMinima(int quantidadeMinima)
    {
        if (quantidadeMinima < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidadeMinima),
                "A quantidade mínima não pode ser negativa.");
        }

        QuantidadeMinima = quantidadeMinima;
        RegistrarAtualizacao();
    }

    private MovimentacaoEstoque Movimentar(
        TipoMovimentacao tipo,
        int quantidade,
        int novoSaldo,
        string? observacao)
    {
        var movimentacao = new MovimentacaoEstoque(tipo, quantidade, Quantidade, novoSaldo, observacao);

        Quantidade = novoSaldo;
        _movimentacoes.Add(movimentacao);
        RegistrarAtualizacao();

        return movimentacao;
    }

    private static void ExigirQuantidadePositiva(int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidade),
                "A quantidade movimentada deve ser maior que zero.");
        }
    }
}
