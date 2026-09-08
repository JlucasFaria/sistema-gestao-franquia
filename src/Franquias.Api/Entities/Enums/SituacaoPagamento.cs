namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Situação de pagamento de uma cobrança de royalty.
/// </summary>
public enum SituacaoPagamento
{
    /// <summary>Cobrança em aberto, dentro do prazo de vencimento.</summary>
    Pendente = 1,

    /// <summary>Cobrança quitada.</summary>
    Pago = 2,

    /// <summary>Cobrança vencida e ainda não quitada.</summary>
    Atrasado = 3
}
