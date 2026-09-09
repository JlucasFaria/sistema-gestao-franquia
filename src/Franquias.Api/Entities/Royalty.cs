using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Cobrança de royalty devida por uma unidade franqueada em um período de apuração.
/// </summary>
public class Royalty : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Royalty()
    {
    }

    public Royalty(
        int unidadeFranqueadaId,
        DateOnly periodoInicio,
        DateOnly periodoFim,
        decimal faturamentoBase,
        decimal percentualAplicado,
        DateOnly dataVencimento)
    {
        if (periodoFim < periodoInicio)
        {
            throw new ArgumentException(
                "O fim do período de apuração não pode ser anterior ao início.",
                nameof(periodoFim));
        }

        UnidadeFranqueadaId = unidadeFranqueadaId;
        PeriodoInicio = periodoInicio;
        PeriodoFim = periodoFim;
        FaturamentoBase = faturamentoBase;
        PercentualAplicado = percentualAplicado;
        DataVencimento = dataVencimento;
        ValorDevido = CalcularValorDevido(faturamentoBase, percentualAplicado);
        Situacao = SituacaoPagamento.Pendente;
    }

    /// <summary>Chave estrangeira da unidade cobrada.</summary>
    public int UnidadeFranqueadaId { get; private set; }

    /// <summary>Unidade cobrada.</summary>
    public UnidadeFranqueada UnidadeFranqueada { get; private set; } = null!;

    /// <summary>Primeiro dia do período de apuração.</summary>
    public DateOnly PeriodoInicio { get; private set; }

    /// <summary>Último dia do período de apuração.</summary>
    public DateOnly PeriodoFim { get; private set; }

    /// <summary>Faturamento apurado no período, base de cálculo da cobrança.</summary>
    public decimal FaturamentoBase { get; private set; }

    /// <summary>
    /// Percentual vigente na unidade no momento da geração. Fica registrado aqui para que
    /// uma renegociação futura não altere cobranças já emitidas.
    /// </summary>
    public decimal PercentualAplicado { get; private set; }

    /// <summary>Valor da cobrança, derivado do faturamento e do percentual.</summary>
    public decimal ValorDevido { get; private set; }

    /// <summary>Data limite para o pagamento.</summary>
    public DateOnly DataVencimento { get; private set; }

    /// <summary>Situação de pagamento da cobrança.</summary>
    public SituacaoPagamento Situacao { get; private set; }

    /// <summary>Data em que a cobrança foi quitada.</summary>
    public DateOnly? DataPagamento { get; private set; }

    /// <summary>Valor efetivamente pago.</summary>
    public decimal? ValorPago { get; private set; }

    /// <summary>
    /// Indica se a cobrança ainda não foi quitada.
    /// </summary>
    public bool EstaEmAberto() => Situacao != SituacaoPagamento.Pago;

    /// <summary>
    /// Dá baixa no pagamento da cobrança.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se a cobrança já estiver quitada.</exception>
    public void RegistrarPagamento(DateOnly dataPagamento, decimal valorPago)
    {
        if (Situacao == SituacaoPagamento.Pago)
        {
            throw new InvalidOperationException("Esta cobrança de royalty já está quitada.");
        }

        if (valorPago <= decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(valorPago),
                "O valor pago deve ser maior que zero.");
        }

        DataPagamento = dataPagamento;
        ValorPago = valorPago;
        Situacao = SituacaoPagamento.Pago;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Marca a cobrança como atrasada quando a data de referência ultrapassa o vencimento.
    /// Não altera cobranças já quitadas.
    /// </summary>
    public void AvaliarAtraso(DateOnly dataReferencia)
    {
        if (Situacao != SituacaoPagamento.Pendente || dataReferencia <= DataVencimento)
        {
            return;
        }

        Situacao = SituacaoPagamento.Atrasado;
        RegistrarAtualizacao();
    }

    private static decimal CalcularValorDevido(decimal faturamentoBase, decimal percentual) =>
        Math.Round(faturamentoBase * percentual / 100m, 2, MidpointRounding.AwayFromZero);
}
