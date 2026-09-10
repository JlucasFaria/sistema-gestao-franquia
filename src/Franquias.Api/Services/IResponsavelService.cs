using Franquias.Api.DTOs.Responsaveis;

namespace Franquias.Api.Services;

/// <summary>
/// Gestão dos responsáveis vinculados a uma unidade franqueada.
/// </summary>
public interface IResponsavelService
{
    /// <summary>Lista os responsáveis de uma unidade.</summary>
    Task<IReadOnlyCollection<ResponsavelResponse>> ListarAsync(
        int unidadeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Vincula um responsável à unidade, recusando CPF já vinculado a ela.
    /// </summary>
    Task<ResponsavelResponse> AdicionarAsync(
        int unidadeId,
        CriarResponsavelRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera cargo e dados de contato de um responsável da unidade.</summary>
    Task<ResponsavelResponse> AtualizarAsync(
        int unidadeId,
        int responsavelId,
        AtualizarResponsavelRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Desvincula um responsável da unidade.</summary>
    Task RemoverAsync(
        int unidadeId,
        int responsavelId,
        CancellationToken cancellationToken = default);
}
