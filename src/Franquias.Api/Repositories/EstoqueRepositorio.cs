using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Estoques;
using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IEstoqueRepositorio"/>.
/// </summary>
public class EstoqueRepositorio(AppDbContext contexto)
    : RepositorioGenerico<Estoque>(contexto), IEstoqueRepositorio
{
    /// <summary>
    /// Sobrescreve a busca por identificador para trazer unidade e item do catálogo, que a
    /// resposta da API exibe.
    /// </summary>
    public override async Task<Estoque?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await ComRelacionamentos(Conjunto)
            .FirstOrDefaultAsync(estoque => estoque.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<Estoque?> ObterPorUnidadeEProdutoAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        CancellationToken cancellationToken = default) =>
        await ComRelacionamentos(Conjunto)
            .FirstOrDefaultAsync(
                estoque => estoque.UnidadeFranqueadaId == unidadeFranqueadaId
                    && estoque.ProdutoServicoId == produtoServicoId,
                cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<Estoque>> ListarAsync(
        QueryParams parametros,
        FiltroEstoquesRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var consulta = ComRelacionamentos(Conjunto.AsNoTracking());

        if (filtro.UnidadeFranqueadaId is not null)
        {
            consulta = consulta.Where(estoque =>
                estoque.UnidadeFranqueadaId == filtro.UnidadeFranqueadaId);
        }

        if (filtro.ProdutoServicoId is not null)
        {
            consulta = consulta.Where(estoque =>
                estoque.ProdutoServicoId == filtro.ProdutoServicoId);
        }

        if (filtro.AbaixoDoMinimo is not null)
        {
            // Reproduz em SQL a regra de Estoque.EstaAbaixoDoMinimo(): o método da entidade
            // não pode ser traduzido pelo EF, então a comparação é repetida aqui.
            consulta = filtro.AbaixoDoMinimo.Value
                ? consulta.Where(estoque => estoque.Quantidade < estoque.QuantidadeMinima)
                : consulta.Where(estoque => estoque.Quantidade >= estoque.QuantidadeMinima);
        }

        if (!string.IsNullOrWhiteSpace(parametros.Busca))
        {
            var termo = $"%{parametros.Busca.Trim()}%";

            consulta = consulta.Where(estoque =>
                EF.Functions.Like(estoque.ProdutoServico.Nome, termo));
        }

        // Sem ordenação pedida, os saldos saem agrupados por unidade e, dentro dela, por
        // item: é como a conferência de estoque é feita na prática.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta
                .OrderBy(estoque => estoque.UnidadeFranqueada.NomeFantasia)
                .ThenBy(estoque => estoque.ProdutoServico.Nome)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<MovimentacaoEstoque>> ListarMovimentacoesAsync(
        int estoqueId,
        QueryParams parametros,
        CancellationToken cancellationToken = default)
    {
        var consulta = Contexto.MovimentacoesEstoque
            .AsNoTracking()
            .Where(movimentacao => movimentacao.EstoqueId == estoqueId);

        // A ordem padrão é a mais recente primeiro. O desempate pelo identificador mantém a
        // paginação estável quando duas movimentações caem no mesmo instante.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta
                .OrderByDescending(movimentacao => movimentacao.DataCriacao)
                .ThenByDescending(movimentacao => movimentacao.Id)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }

    private static IQueryable<Estoque> ComRelacionamentos(IQueryable<Estoque> consulta) =>
        consulta
            .Include(estoque => estoque.UnidadeFranqueada)
            .Include(estoque => estoque.ProdutoServico);
}
