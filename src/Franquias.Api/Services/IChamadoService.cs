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
    /// Abre um chamado em nome de uma unidade. O autor é o usuário autenticado.
    /// </summary>
    /// <param name="requisicao">Unidade, classificação e relato do problema.</param>
    /// <param name="usuarioAberturaId">Usuário que está abrindo o chamado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<ChamadoResponse> AbrirAsync(
        AbrirChamadoRequest requisicao,
        int usuarioAberturaId,
        CancellationToken cancellationToken = default);
}
