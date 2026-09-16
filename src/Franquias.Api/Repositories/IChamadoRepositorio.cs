using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Chamados;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados dos chamados de suporte.
/// </summary>
public interface IChamadoRepositorio : IRepositorio<ChamadoSuporte>
{
    /// <summary>
    /// Lista os chamados de forma paginada, com a unidade, o autor da abertura e a linha do
    /// tempo carregados.
    /// </summary>
    Task<PagedResult<ChamadoSuporte>> ListarAsync(
        QueryParams parametros,
        FiltroChamadosRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista os chamados ainda não encerrados, dos mais urgentes para os menos urgentes e,
    /// dentro da mesma urgência, do mais antigo para o mais recente.
    /// </summary>
    Task<PagedResult<ChamadoSuporte>> ListarEmAbertoAsync(
        QueryParams parametros,
        FiltroChamadosRequest filtro,
        CancellationToken cancellationToken = default);
}
