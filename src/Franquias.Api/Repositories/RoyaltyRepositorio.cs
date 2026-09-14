using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IRoyaltyRepositorio"/>.
/// </summary>
public class RoyaltyRepositorio(AppDbContext contexto)
    : RepositorioGenerico<Royalty>(contexto), IRoyaltyRepositorio
{
    /// <summary>
    /// Sobrescreve a busca por identificador para trazer a unidade, que a resposta exibe.
    /// </summary>
    public override async Task<Royalty?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await Conjunto
            .Include(royalty => royalty.UnidadeFranqueada)
            .FirstOrDefaultAsync(royalty => royalty.Id == id, cancellationToken);

    /// <inheritdoc />
    /// <remarks>
    /// Dois intervalos se sobrepõem quando cada um começa antes de o outro terminar.
    /// </remarks>
    public async Task<bool> ExisteSobreposicaoAsync(
        int unidadeFranqueadaId,
        DateOnly periodoInicio,
        DateOnly periodoFim,
        CancellationToken cancellationToken = default) =>
        await Conjunto
            .AsNoTracking()
            .AnyAsync(
                royalty => royalty.UnidadeFranqueadaId == unidadeFranqueadaId
                    && royalty.PeriodoInicio <= periodoFim
                    && royalty.PeriodoFim >= periodoInicio,
                cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<Royalty>> ListarAsync(
        QueryParams parametros,
        FiltroRoyaltiesRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var consulta = Conjunto
            .AsNoTracking()
            .Include(royalty => royalty.UnidadeFranqueada)
            .AsQueryable();

        if (filtro.UnidadeFranqueadaId is not null)
        {
            consulta = consulta.Where(royalty => royalty.UnidadeFranqueadaId == filtro.UnidadeFranqueadaId);
        }

        if (filtro.Situacao is not null)
        {
            consulta = consulta.Where(royalty => royalty.Situacao == filtro.Situacao);
        }

        // Sem ordenação pedida, a competência mais recente primeiro e, dentro dela, por unidade.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta
                .OrderByDescending(royalty => royalty.PeriodoInicio)
                .ThenBy(royalty => royalty.UnidadeFranqueada.NomeFantasia)
                .ThenByDescending(royalty => royalty.Id)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Vencida é a cobrança cujo vencimento ficou para trás: no próprio dia do vencimento ela
    /// ainda está em dia. É a mesma regra de <see cref="Royalty.AvaliarAtraso"/>.
    /// </remarks>
    public async Task<IReadOnlyList<Royalty>> ListarPendentesVencidasAsync(
        DateOnly dataReferencia,
        CancellationToken cancellationToken = default) =>
        await Conjunto
            .Where(royalty => royalty.Situacao == SituacaoPagamento.Pendente
                && royalty.DataVencimento < dataReferencia)
            .ToListAsync(cancellationToken);
}
