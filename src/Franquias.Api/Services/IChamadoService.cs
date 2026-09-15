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

    /// <summary>
    /// Acrescenta uma mensagem à linha do tempo de um chamado em aberto.
    /// </summary>
    /// <param name="id">Chamado que recebe a mensagem.</param>
    /// <param name="requisicao">Conteúdo da mensagem.</param>
    /// <param name="usuarioId">Autor da mensagem.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<ChamadoResponse> RegistrarInteracaoAsync(
        int id,
        RegistrarInteracaoRequest requisicao,
        int usuarioId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Avança o chamado para outro estágio de atendimento. Encerrar carimba a data de
    /// encerramento e impede novas alterações.
    /// </summary>
    /// <param name="id">Chamado que muda de estágio.</param>
    /// <param name="requisicao">Novo status e observação opcional.</param>
    /// <param name="usuarioId">Autor da mudança, registrado quando há observação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<ChamadoResponse> AlterarStatusAsync(
        int id,
        AlterarStatusChamadoRequest requisicao,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
