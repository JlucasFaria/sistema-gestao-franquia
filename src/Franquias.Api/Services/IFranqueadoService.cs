using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Franqueados;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio dos franqueados, as pessoas que firmam o contrato de franquia.
/// </summary>
public interface IFranqueadoService
{
    /// <summary>Lista os franqueados de forma paginada, com busca por nome ou CPF.</summary>
    Task<PagedResult<FranqueadoResponse>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default);

    /// <summary>Busca um franqueado pelo identificador.</summary>
    Task<FranqueadoResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Cadastra um franqueado, recusando CPF já em uso.</summary>
    Task<FranqueadoResponse> CriarAsync(
        CriarFranqueadoRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera os dados de contato de um franqueado. O CPF é imutável.</summary>
    Task<FranqueadoResponse> AtualizarAsync(
        int id,
        AtualizarFranqueadoRequest requisicao,
        CancellationToken cancellationToken = default);
}
