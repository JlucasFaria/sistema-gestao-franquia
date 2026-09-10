using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Perfis;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IPerfilService"/>.
/// </summary>
public sealed class PerfilService(IRepositorio<Perfil> perfis) : IPerfilService
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PerfilResponse>> ListarAsync(
        CancellationToken cancellationToken = default)
    {
        var registros = await perfis.ListarAsync(cancellationToken);

        return [.. registros.OrderBy(perfil => perfil.Codigo).Select(PerfilResponse.De)];
    }

    /// <inheritdoc />
    public async Task<PerfilResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var perfil = await perfis.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Perfil de acesso", id);

        return PerfilResponse.De(perfil);
    }
}
