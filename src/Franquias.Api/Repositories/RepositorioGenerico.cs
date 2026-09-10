using System.Linq.Expressions;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação padrão de <see cref="IRepositorio{T}"/> sobre o Entity Framework Core.
/// Serve como classe base dos repositórios específicos.
/// </summary>
/// <typeparam name="T">Entidade de domínio derivada de <see cref="EntidadeBase"/>.</typeparam>
public class RepositorioGenerico<T>(AppDbContext contexto) : IRepositorio<T>
    where T : EntidadeBase
{
    /// <summary>Contexto de persistência, disponível para os repositórios derivados.</summary>
    protected AppDbContext Contexto { get; } = contexto;

    /// <summary>Conjunto da entidade mapeada.</summary>
    protected DbSet<T> Conjunto => Contexto.Set<T>();

    /// <inheritdoc />
    public virtual async Task<T?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        await Conjunto.FirstOrDefaultAsync(entidade => entidade.Id == id, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<T>> ListarAsync(CancellationToken cancellationToken = default) =>
        await Consultar().ToListAsync(cancellationToken);

    /// <inheritdoc />
    public virtual async Task<PagedResult<T>> ListarPaginadoAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default) =>
        await Consultar().Ordenar(parametros).PaginarAsync(parametros, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<bool> ExisteAsync(
        Expression<Func<T, bool>> predicado,
        CancellationToken cancellationToken = default) =>
        await Conjunto.AsNoTracking().AnyAsync(predicado, cancellationToken);

    /// <inheritdoc />
    public virtual async Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default) =>
        await Conjunto.AddAsync(entidade, cancellationToken);

    /// <inheritdoc />
    public virtual void Atualizar(T entidade) => Conjunto.Update(entidade);

    /// <inheritdoc />
    public virtual void Remover(T entidade) => Conjunto.Remove(entidade);

    /// <inheritdoc />
    public virtual Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        Contexto.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public virtual IQueryable<T> Consultar(bool rastrear = false) =>
        rastrear ? Conjunto : Conjunto.AsNoTracking();
}
