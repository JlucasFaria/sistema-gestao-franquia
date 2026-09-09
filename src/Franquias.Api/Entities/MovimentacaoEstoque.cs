using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Registro histórico de uma alteração no saldo de estoque. Cada movimentação é imutável:
/// o histórico só cresce, nunca é reescrito.
/// </summary>
public class MovimentacaoEstoque : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected MovimentacaoEstoque()
    {
    }

    internal MovimentacaoEstoque(
        TipoMovimentacao tipo,
        int quantidade,
        int saldoAnterior,
        int saldoResultante,
        string? observacao)
    {
        Tipo = tipo;
        Quantidade = quantidade;
        SaldoAnterior = saldoAnterior;
        SaldoResultante = saldoResultante;
        Observacao = observacao;
    }

    /// <summary>Chave estrangeira do saldo de estoque movimentado.</summary>
    public int EstoqueId { get; private set; }

    /// <summary>Saldo de estoque movimentado.</summary>
    public Estoque Estoque { get; private set; } = null!;

    /// <summary>Natureza da movimentação.</summary>
    public TipoMovimentacao Tipo { get; private set; }

    /// <summary>Quantidade movimentada, sempre positiva. O sentido é dado por <see cref="Tipo"/>.</summary>
    public int Quantidade { get; private set; }

    /// <summary>Saldo antes da movimentação.</summary>
    public int SaldoAnterior { get; private set; }

    /// <summary>Saldo depois da movimentação.</summary>
    public int SaldoResultante { get; private set; }

    /// <summary>Justificativa da movimentação, obrigatória na prática para ajustes de inventário.</summary>
    public string? Observacao { get; private set; }
}
