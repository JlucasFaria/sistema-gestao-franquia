using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
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

        if (filtro.Status is not null)
        {
            consulta = consulta.Where(produto => produto.Status == filtro.Status);
        }

        if (filtro.Ativo is not null)
        {
            consulta = consulta.Where(produto => produto.Ativo == filtro.Ativo);
        }

        if (filtro.DisponivelParaVenda is not null)
        {
            // Reproduz em SQL a regra de ProdutoServico.EstaDisponivelParaVenda(): o método
            // da entidade não pode ser traduzido pelo EF, então a condição é repetida aqui.
            consulta = filtro.DisponivelParaVenda.Value
                ? consulta.Where(produto => produto.Ativo && produto.Status == StatusProduto.Ativo)
                : consulta.Where(produto => !produto.Ativo || produto.Status != StatusProduto.Ativo);
        }

        if (!string.IsNullOrWhiteSpace(parametros.Busca))
        {
            var termo = $"%{parametros.Busca.Trim()}%";

            consulta = consulta.Where(produto => EF.Functions.Like(produto.Nome, termo));
        }

        // Sem ordenação pedida, o catálogo sai por nome: ordem estável entre as páginas.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta.OrderBy(produto => produto.Nome)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }
}
