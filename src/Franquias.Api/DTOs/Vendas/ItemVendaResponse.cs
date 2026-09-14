using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Vendas;

/// <summary>
/// Item de uma venda devolvido pela API.
/// </summary>
/// <param name="Id">Identificador do item.</param>
/// <param name="ProdutoServicoId">Item do catálogo vendido.</param>
/// <param name="Produto">Nome do item.</param>
/// <param name="Quantidade">Quantidade vendida.</param>
/// <param name="PrecoUnitario">Preço unitário praticado na venda.</param>
/// <param name="Subtotal">Quantidade vezes preço unitário.</param>
public sealed record ItemVendaResponse(
    int Id,
    int ProdutoServicoId,
    string Produto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal)
{
    /// <summary>Projeta a entidade no DTO de saída. Espera o item do catálogo carregado.</summary>
    public static ItemVendaResponse De(ItemVenda item) => new(
        item.Id,
        item.ProdutoServicoId,
        item.ProdutoServico?.Nome ?? string.Empty,
        item.Quantidade,
        item.PrecoUnitario,
        item.Subtotal);
}
