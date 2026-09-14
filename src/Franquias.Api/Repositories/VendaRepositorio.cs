using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Vendas;
using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IVendaRepositorio"/>.
/// </summary>
public class VendaRepositorio(AppDbContext contexto)
    : RepositorioGenerico<Venda>(contexto), IVendaRepositorio
{
    /// <summary>
    /// Sobrescreve a busca por identificador para trazer a unidade e os itens com seus
    /// produtos: confirmar e cancelar a venda dependem dos itens para movimentar o estoque.
    /// </summary>
    public override async Task<Venda?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await ComRelacionamentos(Conjunto)
            .FirstOrDefaultAsync(venda => venda.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<Venda>> ListarAsync(
        QueryParams parametros,
        FiltroVendasRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var consulta = ComRelacionamentos(Conjunto.AsNoTracking());

        if (filtro.UnidadeFranqueadaId is not null)
        {
            consulta = consulta.Where(venda => venda.UnidadeFranqueadaId == filtro.UnidadeFranqueadaId);
        }

        if (filtro.Status is not null)
        {
            consulta = consulta.Where(venda => venda.Status == filtro.Status);
        }

        if (filtro.DataInicial is not null)
        {
            var inicio = filtro.DataInicial.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            consulta = consulta.Where(venda => venda.DataVenda >= inicio);
        }

        if (filtro.DataFinal is not null)
        {
            // A data final é inclusiva: o limite é o primeiro instante do dia seguinte, com
            // comparação estrita. Comparar com a própria data final à meia-noite deixaria de
            // fora todas as vendas feitas ao longo do último dia.
            var fimExclusivo = filtro.DataFinal.Value
                .AddDays(1)
                .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            consulta = consulta.Where(venda => venda.DataVenda < fimExclusivo);
        }

        // Sem ordenação pedida, a mais recente primeiro. O desempate pelo identificador mantém
        // a paginação estável quando duas vendas caem no mesmo instante.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta
                .OrderByDescending(venda => venda.DataVenda)
                .ThenByDescending(venda => venda.Id)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }

    private static IQueryable<Venda> ComRelacionamentos(IQueryable<Venda> consulta) =>
        consulta
            .Include(venda => venda.UnidadeFranqueada)
            .Include(venda => venda.Itens)
                .ThenInclude(item => item.ProdutoServico);
}
