using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;

namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Baixa de pagamento de uma cobrança de royalty.
/// </summary>
public class RegistrarPagamentoRequest
{
    /// <summary>Data em que o pagamento foi efetuado. Não pode estar no futuro.</summary>
    [Required(ErrorMessage = "A data do pagamento é obrigatória.")]
    public DateOnly? DataPagamento { get; set; }

    /// <summary>
    /// Valor efetivamente pago. Precisa cobrir o valor devido; valor maior é aceito, para
    /// contemplar multa e juros de atraso.
    /// </summary>
    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "O valor pago deve ser maior que zero.")]
    [CasasDecimais(2)]
    public decimal ValorPago { get; set; }
}
