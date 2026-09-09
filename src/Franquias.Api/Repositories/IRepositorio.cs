using System.Linq.Expressions;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Contrato de acesso a dados comum a todas as entidades do domínio. Os repositórios
/// específicos herdam este conjunto e acrescentam apenas as consultas próprias da entidade.
/// </summary>
/// <typeparam name="T">Entidade de domínio derivada de <see cref="EntidadeBase"/>.</typeparam>
public interface IRepositorio<T>
    where T : EntidadeBase
{
    /// <summary>Busca uma entidade pela chave primária. Devolve nulo se não existir.</summary>
    Task<T?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Lista todas as entidades. Indicado apenas para conjuntos pequenos, como perfis.</summary>
    Task<IReadOnlyCollection<T>> ListarAsync(CancellationToken cancellationToken = default);

    /// <summary>Lista as entidades de forma paginada e ordenada.</summary>
    Task<PagedResult<T>> ListarPaginadoAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica a existência de algum registro que satisfaça o predicado, sem materializá-lo.
    /// É o que sustenta as regras de e-mail e CNPJ únicos.
    /// </summary>
    Task<bool> ExisteAsync(
        Expression<Func<T, bool>> predicado,
        CancellationToken cancellationToken = default);

    /// <summary>Marca uma entidade nova para inserção.</summary>
    Task AdicionarAsync(T entidade, CancellationToken cancellationToken = default);

    /// <summary>Marca uma entidade existente como alterada.</summary>
    void Atualizar(T entidade);

    /// <summary>
    /// Marca uma entidade para exclusão física. O sistema usa exclusão lógica via
    /// <c>Inativar()</c>; este método existe para os poucos casos em que remover de fato
    /// é o correto, como desfazer um vínculo entre fornecedor e produto.
    /// </summary>
    void Remover(T entidade);

    /// <summary>
    /// Persiste as alterações pendentes. Como todos os repositórios compartilham o mesmo
    /// <c>AppDbContext</c> por requisição, uma única chamada confirma o trabalho de todos
    /// eles em conjunto.
    /// </summary>
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta base para os repositórios específicos comporem filtros e projeções.
    /// Por padrão não rastreia as entidades, já que a maioria das consultas é só leitura.
    /// </summary>
    IQueryable<T> Consultar(bool rastrear = false);
}
