using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IUsuarioRepositorio"/>.
/// </summary>
public class UsuarioRepositorio(AppDbContext contexto)
    : RepositorioGenerico<Usuario>(contexto), IUsuarioRepositorio
{
    /// <summary>
    /// Sobrescreve a busca por identificador para trazer o perfil junto: praticamente toda
    /// leitura de usuário precisa do nome do perfil, e sem o <c>Include</c> a projeção para
    /// DTO devolveria o campo vazio.
    /// </summary>
    public override async Task<Usuario?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await Conjunto
            .Include(usuario => usuario.Perfil)
            .FirstOrDefaultAsync(usuario => usuario.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<Usuario?> ObterPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizado = NormalizarEmail(email);

        return await Conjunto
            .Include(usuario => usuario.Perfil)
            .FirstOrDefaultAsync(usuario => usuario.Email == normalizado, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExisteComEmailAsync(
        string email,
        int? idIgnorado = null,
        CancellationToken cancellationToken = default)
    {
        var normalizado = NormalizarEmail(email);

        return await Conjunto
            .AsNoTracking()
            .AnyAsync(
                usuario => usuario.Email == normalizado
                    && (idIgnorado == null || usuario.Id != idIgnorado),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Usuario>> ListarAsync(
        QueryParams parametros,
        bool? apenasAtivos,
        CancellationToken cancellationToken = default)
    {
        var consulta = Conjunto
            .AsNoTracking()
            .Include(usuario => usuario.Perfil)
            .AsQueryable();

        if (apenasAtivos is not null)
        {
            consulta = consulta.Where(usuario => usuario.Ativo == apenasAtivos);
        }

        if (!string.IsNullOrWhiteSpace(parametros.Busca))
        {
            var termo = parametros.Busca.Trim();

            consulta = consulta.Where(usuario =>
                EF.Functions.Like(usuario.Nome, $"%{termo}%")
                || EF.Functions.Like(usuario.Email, $"%{termo}%"));
        }

        // Sem ordenação pedida, a listagem sai por nome: ordem estável entre páginas, o que
        // a ordem natural do banco não garante.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta.OrderBy(usuario => usuario.Nome)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> ContarAdministradoresAtivosAsync(
        CancellationToken cancellationToken = default) =>
        await Conjunto
            .AsNoTracking()
            .CountAsync(
                usuario => usuario.Ativo
                    && usuario.Perfil.Codigo == PerfilAcesso.Administrador,
                cancellationToken);

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
