namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Estágio de uma venda registrada por uma unidade.
/// </summary>
public enum StatusVenda
{
    /// <summary>Venda registrada, ainda sem baixa no estoque.</summary>
    Pendente = 1,

    /// <summary>Venda concluída, com o estoque já baixado.</summary>
    Confirmada = 2,

    /// <summary>Venda cancelada; as quantidades foram estornadas ao estoque.</summary>
    Cancelada = 3
}
