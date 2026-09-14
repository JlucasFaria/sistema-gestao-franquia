using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados das cobranças de royalty.
/// </summary>
public interface IRoyaltyRepositorio : IRepositorio<Royalty>
{
    /// <summary>
    /// Indica se a unidade já tem cobrança cujo período se sobrepõe ao informado, mesmo que
    /// parcialmente. O índice único do banco só impede períodos idênticos; esta checagem
    /// impede também que o mesmo dia de faturamento seja cobrado duas vezes.
    /// </summary>
    Task<bool> ExisteSobreposicaoAsync(
        int unidadeFranqueadaId,
        DateOnly periodoInicio,
        DateOnly periodoFim,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista as cobranças de forma paginada, com a unidade carregada, filtrando por unidade
    /// e situação de pagamento.
    /// </summary>
    Task<PagedResult<Royalty>> ListarAsync(
        QueryParams parametros,
        FiltroRoyaltiesRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca, rastreadas para atualização, as cobranças ainda pendentes cujo vencimento é
    /// anterior à data de referência.
    /// </summary>
    Task<IReadOnlyList<Royalty>> ListarPendentesVencidasAsync(
        DateOnly dataReferencia,
        CancellationToken cancellationToken = default);
}
