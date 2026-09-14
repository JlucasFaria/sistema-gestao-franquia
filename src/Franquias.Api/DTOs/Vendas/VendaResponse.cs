using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Vendas;

/// <summary>
/// Venda devolvida pela API.
/// </summary>
/// <param name="Id">Identificador da venda.</param>
/// <param name="UnidadeFranqueadaId">Unidade que realizou a venda.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="DataVenda">Momento da venda, em UTC.</param>
/// <param name="Status">Estágio da venda: Pendente, Confirmada ou Cancelada.</param>
/// <param name="ValorTotal">Soma dos subtotais dos itens.</param>
/// <param name="Itens">Itens que compõem a venda.</param>
/// <param name="DataCriacao">Momento do registro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última mudança de estágio, em UTC.</param>
public sealed record VendaResponse(
    int Id,
    int UnidadeFranqueadaId,
    string Unidade,
    DateTime DataVenda,
    StatusVenda Status,
    decimal ValorTotal,
    IReadOnlyCollection<ItemVendaResponse> Itens,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>
    /// Projeta a entidade no DTO de saída. Espera a unidade e os itens, com seus produtos,
    /// carregados.
    /// </summary>
    public static VendaResponse De(Venda venda) => new(
        venda.Id,
        venda.UnidadeFranqueadaId,
        venda.UnidadeFranqueada?.NomeFantasia ?? string.Empty,
        venda.DataVenda,
        venda.Status,
        venda.ValorTotal,
        [.. venda.Itens.OrderBy(item => item.Id).Select(ItemVendaResponse.De)],
        venda.DataCriacao,
        venda.DataAtualizacao);
}
