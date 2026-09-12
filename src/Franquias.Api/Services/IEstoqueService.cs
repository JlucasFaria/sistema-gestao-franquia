using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Estoques;

namespace Franquias.Api.Services;

/// <summary>
/// Consulta dos saldos de estoque por unidade franqueada.
/// </summary>
public interface IEstoqueService
{
    /// <summary>
    /// Lista os saldos de forma paginada, com busca por nome do item e os filtros informados.
    /// </summary>
    Task<PagedResult<EstoqueResponse>> ListarAsync(
        QueryParams parametros,
        FiltroEstoquesRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca o saldo de um item em uma unidade.
    /// </summary>
    /// <param name="unidadeFranqueadaId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <exception cref="Common.Excecoes.NaoEncontradoException">
    /// Se a unidade ou o item não existirem, ou se o item nunca tiver sido controlado na
    /// unidade. Saldo zero e ausência de controle são situações distintas.
    /// </exception>
    Task<EstoqueResponse> ObterSaldoAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        CancellationToken cancellationToken = default);
}
