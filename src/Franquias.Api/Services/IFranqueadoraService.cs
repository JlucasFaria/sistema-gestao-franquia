using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Franqueadoras;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio da franqueadora, a empresa dona da marca e da rede.
/// </summary>
public interface IFranqueadoraService
{
    /// <summary>Lista as franqueadoras de forma paginada.</summary>
    Task<PagedResult<FranqueadoraResponse>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default);

    /// <summary>Busca uma franqueadora pelo identificador.</summary>
    Task<FranqueadoraResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Cadastra uma franqueadora, recusando CNPJ já em uso.</summary>
    Task<FranqueadoraResponse> CriarAsync(
        CriarFranqueadoraRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera os dados cadastrais de uma franqueadora. O CNPJ é imutável.</summary>
    Task<FranqueadoraResponse> AtualizarAsync(
        int id,
        AtualizarFranqueadoraRequest requisicao,
        CancellationToken cancellationToken = default);
}
