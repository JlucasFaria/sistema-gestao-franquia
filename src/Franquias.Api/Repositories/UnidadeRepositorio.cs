using Franquias.Api.Common;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IUnidadeRepositorio"/>.
/// </summary>
public class UnidadeRepositorio(AppDbContext contexto)
    : RepositorioGenerico<UnidadeFranqueada>(contexto), IUnidadeRepositorio
{
    /// <summary>
    /// Sobrescreve a busca por identificador para trazer franqueadora, franqueado e
    /// responsáveis: a resposta da API exibe os três, e a gestão de responsáveis precisa
    /// da coleção carregada para aplicar as regras do agregado.
    /// </summary>
    public override async Task<UnidadeFranqueada?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await ComRelacionamentos(Conjunto)
            .FirstOrDefaultAsync(unidade => unidade.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExisteComCnpjAsync(
        string cnpj,
        int? idIgnorado = null,
        CancellationToken cancellationToken = default)
    {
        var normalizado = cnpj.SomenteDigitos();

        return await Conjunto
            .AsNoTracking()
            .AnyAsync(
                unidade => unidade.Cnpj == normalizado
                    && (idIgnorado == null || unidade.Id != idIgnorado),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<UnidadeFranqueada>> ListarAsync(
        QueryParams parametros,
        FiltroUnidadesRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var consulta = ComRelacionamentos(Conjunto.AsNoTracking());

        if (filtro.Situacao is not null)
        {
            consulta = consulta.Where(unidade => unidade.Situacao == filtro.Situacao);
        }

        if (filtro.FranqueadoraId is not null)
        {
            consulta = consulta.Where(unidade => unidade.FranqueadoraId == filtro.FranqueadoraId);
        }

        if (filtro.FranqueadoId is not null)
        {
            consulta = consulta.Where(unidade => unidade.FranqueadoId == filtro.FranqueadoId);
        }

        if (!string.IsNullOrWhiteSpace(parametros.Busca))
        {
            var termo = $"%{parametros.Busca.Trim()}%";

            consulta = consulta.Where(unidade =>
                EF.Functions.Like(unidade.NomeFantasia, termo)
                || EF.Functions.Like(unidade.RazaoSocial, termo));
        }

        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta.OrderBy(unidade => unidade.NomeFantasia)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }

    private static IQueryable<UnidadeFranqueada> ComRelacionamentos(IQueryable<UnidadeFranqueada> consulta) =>
        consulta
            .Include(unidade => unidade.Franqueadora)
            .Include(unidade => unidade.Franqueado)
            .Include(unidade => unidade.Responsaveis);
}
