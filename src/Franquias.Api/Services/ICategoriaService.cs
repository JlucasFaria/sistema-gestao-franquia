using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Categorias;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio das categorias do catálogo.
/// </summary>
public interface ICategoriaService
{
    /// <summary>Lista as categorias de forma paginada, com busca por nome.</summary>
    /// <param name="parametros">Paginação, ordenação e termo de busca.</param>
    /// <param name="apenasAtivas">Filtra por situação. Nulo traz ativas e inativas.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<PagedResult<CategoriaResponse>> ListarAsync(
        QueryParams parametros,
        bool? apenasAtivas,
        CancellationToken cancellationToken = default);

    /// <summary>Busca uma categoria pelo identificador.</summary>
    Task<CategoriaResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Cadastra uma categoria, recusando nome já em uso.</summary>
    Task<CategoriaResponse> CriarAsync(
        CategoriaRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera uma categoria, recusando nome já em uso por outra.</summary>
    Task<CategoriaResponse> AtualizarAsync(
        int id,
        CategoriaRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Reativa uma categoria. Operação idempotente.</summary>
    Task<CategoriaResponse> AtivarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inativa uma categoria: ela deixa de aceitar novos itens, mas os existentes continuam
    /// vinculados. Operação idempotente.
    /// </summary>
    Task<CategoriaResponse> InativarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui uma categoria. Só é permitido enquanto nenhum item do catálogo estiver
    /// vinculado a ela.
    /// </summary>
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
