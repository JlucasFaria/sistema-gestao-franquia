using System.Linq.Expressions;
using System.Reflection;
using Franquias.Api.Common.Excecoes;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Common.Consultas;

/// <summary>
/// Extensões que aplicam ordenação e paginação sobre uma consulta, mantendo tudo do lado
/// do banco: nada é materializado antes de a página ser recortada.
/// </summary>
public static class ExtensoesDeConsulta
{
    /// <summary>
    /// Ordena pela propriedade indicada em <see cref="QueryParams.OrdenarPor"/>. Sem campo
    /// informado, a consulta segue inalterada.
    /// </summary>
    /// <exception cref="RegraDeNegocioException">Se a propriedade não existir no tipo.</exception>
    public static IQueryable<T> Ordenar<T>(this IQueryable<T> consulta, QueryParams parametros)
    {
        if (string.IsNullOrWhiteSpace(parametros.OrdenarPor))
        {
            return consulta;
        }

        var propriedade = typeof(T).GetProperty(
            parametros.OrdenarPor,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (propriedade is null)
        {
            throw new RegraDeNegocioException(
                $"Não é possível ordenar por '{parametros.OrdenarPor}': a propriedade não existe.");
        }

        // Monta e => e.Propriedade e chama Queryable.OrderBy/OrderByDescending com ela.
        var parametro = Expression.Parameter(typeof(T), "e");
        var acesso = Expression.MakeMemberAccess(parametro, propriedade);
        var seletor = Expression.Lambda(acesso, parametro);

        var metodo = parametros.Decrescente
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        var chamada = Expression.Call(
            typeof(Queryable),
            metodo,
            [typeof(T), propriedade.PropertyType],
            consulta.Expression,
            Expression.Quote(seletor));

        return consulta.Provider.CreateQuery<T>(chamada);
    }

    /// <summary>
    /// Executa a consulta em duas idas ao banco — a contagem total e o recorte da página —
    /// e devolve o resultado paginado.
    /// </summary>
    public static async Task<PagedResult<T>> PaginarAsync<T>(
        this IQueryable<T> consulta,
        QueryParams parametros,
        CancellationToken cancellationToken = default)
    {
        var total = await consulta.CountAsync(cancellationToken);

        if (total == 0)
        {
            return PagedResult<T>.Vazia(parametros);
        }

        var itens = await consulta
            .Skip(parametros.QuantidadeParaPular())
            .Take(parametros.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(itens, total, parametros.Pagina, parametros.TamanhoPagina);
    }
}
