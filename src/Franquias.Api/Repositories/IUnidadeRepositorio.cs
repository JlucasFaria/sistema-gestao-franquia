using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados de unidades franqueadas.
/// </summary>
public interface IUnidadeRepositorio : IRepositorio<UnidadeFranqueada>
{
    /// <summary>
    /// Indica se o CNPJ já pertence a alguma unidade. O valor é normalizado antes da
    /// comparação, de modo que máscaras diferentes não escapem da checagem.
    /// </summary>
    /// <param name="cnpj">CNPJ a verificar, com ou sem máscara.</param>
    /// <param name="idIgnorado">Unidade a desconsiderar na checagem.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<bool> ExisteComCnpjAsync(
        string cnpj,
        int? idIgnorado = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista as unidades de forma paginada, com franqueadora, franqueado e responsáveis
    /// carregados, aplicando os filtros informados.
    /// </summary>
    Task<PagedResult<UnidadeFranqueada>> ListarAsync(
        QueryParams parametros,
        FiltroUnidadesRequest filtro,
        CancellationToken cancellationToken = default);
}
