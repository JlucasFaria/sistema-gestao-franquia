using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio das unidades franqueadas.
/// </summary>
public interface IUnidadeService
{
    /// <summary>Lista as unidades de forma paginada, aplicando os filtros informados.</summary>
    Task<PagedResult<UnidadeResponse>> ListarAsync(
        QueryParams parametros,
        FiltroUnidadesRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>Busca uma unidade pelo identificador.</summary>
    Task<UnidadeResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cadastra uma unidade, recusando CNPJ já em uso. A unidade nasce em implantação.
    /// </summary>
    Task<UnidadeResponse> CriarAsync(
        CriarUnidadeRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera os dados cadastrais de uma unidade. O CNPJ é imutável.</summary>
    Task<UnidadeResponse> AtualizarAsync(
        int id,
        AtualizarUnidadeRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera a situação contratual de uma unidade.</summary>
    Task<UnidadeResponse> AlterarSituacaoAsync(
        int id,
        SituacaoUnidade situacao,
        CancellationToken cancellationToken = default);

    /// <summary>Reativa o cadastro de uma unidade. Operação idempotente.</summary>
    Task<UnidadeResponse> AtivarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inativa o cadastro de uma unidade, sem removê-la: vendas, estoque e royalties
    /// continuam vinculados a ela. Operação idempotente.
    /// </summary>
    Task<UnidadeResponse> InativarAsync(int id, CancellationToken cancellationToken = default);
}
