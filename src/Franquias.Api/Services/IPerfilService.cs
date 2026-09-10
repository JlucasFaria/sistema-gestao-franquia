using Franquias.Api.DTOs.Perfis;

namespace Franquias.Api.Services;

/// <summary>
/// Consulta dos perfis de acesso da rede.
/// </summary>
public interface IPerfilService
{
    /// <summary>
    /// Lista todos os perfis. O conjunto é fixo e pequeno — um por valor de
    /// <c>PerfilAcesso</c> —, por isso não é paginado.
    /// </summary>
    Task<IReadOnlyCollection<PerfilResponse>> ListarAsync(CancellationToken cancellationToken = default);

    /// <summary>Busca um perfil pelo identificador.</summary>
    Task<PerfilResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
}
