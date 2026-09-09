namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Natureza de uma movimentação no estoque de uma unidade.
/// </summary>
public enum TipoMovimentacao
{
    /// <summary>Acréscimo de quantidade, por compra, transferência ou estorno de venda.</summary>
    Entrada = 1,

    /// <summary>Redução de quantidade, por venda, perda ou transferência.</summary>
    Saida = 2,

    /// <summary>Correção do saldo após conferência física.</summary>
    Ajuste = 3
}
