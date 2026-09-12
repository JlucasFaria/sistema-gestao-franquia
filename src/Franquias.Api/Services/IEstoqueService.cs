using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Estoques;

namespace Franquias.Api.Services;

/// <summary>
/// Consulta e movimentação dos saldos de estoque por unidade franqueada.
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

    /// <summary>
    /// Dá entrada no estoque. A primeira entrada de um item abre o controle dele na unidade.
    /// </summary>
    Task<EstoqueResponse> RegistrarEntradaAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        MovimentacaoRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Dá baixa no estoque, recusando a operação quando o saldo é insuficiente.
    /// </summary>
    Task<EstoqueResponse> RegistrarSaidaAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        MovimentacaoRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Corrige o saldo para a quantidade apurada em contagem física.
    /// </summary>
    Task<EstoqueResponse> AjustarAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        AjusteEstoqueRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Define o ponto de reposição do item na unidade. Abre o controle com saldo zero caso
    /// o item ainda não seja controlado ali.
    /// </summary>
    Task<EstoqueResponse> DefinirQuantidadeMinimaAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        DefinirQuantidadeMinimaRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista o histórico de movimentações de um item na unidade, da mais recente para a
    /// mais antiga.
    /// </summary>
    Task<PagedResult<MovimentacaoEstoqueResponse>> ListarMovimentacoesAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        QueryParams parametros,
        CancellationToken cancellationToken = default);
}
