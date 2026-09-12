using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Estoques;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados dos saldos de estoque e do respectivo histórico de movimentações.
/// </summary>
public interface IEstoqueRepositorio : IRepositorio<Estoque>
{
    /// <summary>
    /// Busca o saldo de um item em uma unidade. É a forma natural de acessar o estoque: o
    /// par unidade e item é único, garantido por índice na base.
    /// </summary>
    /// <param name="unidadeFranqueadaId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<Estoque?> ObterPorUnidadeEProdutoAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista os saldos de forma paginada, com unidade e item carregados, aplicando busca por
    /// nome do item e os filtros informados.
    /// </summary>
    Task<PagedResult<Estoque>> ListarAsync(
        QueryParams parametros,
        FiltroEstoquesRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista o histórico de movimentações de um saldo, da mais recente para a mais antiga.
    /// </summary>
    Task<PagedResult<MovimentacaoEstoque>> ListarMovimentacoesAsync(
        int estoqueId,
        QueryParams parametros,
        CancellationToken cancellationToken = default);
}
