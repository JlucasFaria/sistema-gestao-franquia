using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Chamados;

namespace Franquias.Api.Services;

/// <summary>
/// Atendimento dos chamados de suporte abertos pelas unidades franqueadas.
/// </summary>
public interface IChamadoService
{
    /// <summary>Busca um chamado pelo identificador, com a linha do tempo do atendimento.</summary>
    Task<ChamadoResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista os chamados de forma paginada, filtrando por unidade, categoria, prioridade,
    /// status e por estarem ou não em aberto.
    /// </summary>
    Task<PagedResult<ChamadoResponse>> ListarAsync(
        QueryParams parametros,
        FiltroChamadosRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Abre um chamado em nome de uma unidade. O autor é o usuário autenticado.
    /// </summary>
    /// <param name="requisicao">Unidade, classificação e relato do problema.</param>
    /// <param name="usuarioAberturaId">Usuário que está abrindo o chamado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<ChamadoResponse> AbrirAsync(
        AbrirChamadoRequest requisicao,
        int usuarioAberturaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reclassifica a urgência de um chamado que ainda não foi encerrado.
    /// </summary>
    Task<ChamadoResponse> AlterarPrioridadeAsync(
        int id,
        AlterarPrioridadeChamadoRequest requisicao,
        CancellationToken cancellationToken = default);
}
