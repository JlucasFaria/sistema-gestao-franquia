using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IProdutoServicoRepositorio"/>.
/// </summary>
public class ProdutoServicoRepositorio(AppDbContext contexto)
    : RepositorioGenerico<ProdutoServico>(contexto), IProdutoServicoRepositorio
{
    /// <summary>
    /// Sobrescreve a busca por identificador para trazer a categoria: toda resposta da API
    /// exibe o nome dela, e sem o <c>Include</c> o campo sairia vazio.
    /// </summary>
    public override async Task<ProdutoServico?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await Conjunto
            .Include(produto => produto.Categoria)
            .FirstOrDefaultAsync(produto => produto.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<ProdutoServico>> ListarAsync(
        QueryParams parametros,
        FiltroProdutosRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var consulta = Conjunto
            .AsNoTracking()
            .Include(produto => produto.Categoria)
            .AsQueryable();

        if (filtro.CategoriaId is not null)
        {
            consulta = consulta.Where(produto => produto.CategoriaId == filtro.CategoriaId);
        }

        if (filtro.EhServico is not null)
        {
            consulta = consulta.Where(produto => produto.EhServico == filtro.EhServico);
        }

        // Sem ordenação pedida, o catálogo sai por nome: ordem estável entre as páginas.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta.OrderBy(produto => produto.Nome)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }
}
