using Franquias.Api.Common;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IFornecedorRepositorio"/>.
/// </summary>
public class FornecedorRepositorio(AppDbContext contexto)
    : RepositorioGenerico<Fornecedor>(contexto), IFornecedorRepositorio
{
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
                fornecedor => fornecedor.Cnpj == normalizado
                    && (idIgnorado == null || fornecedor.Id != idIgnorado),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Fornecedor?> ObterComProdutosAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await Conjunto
            .Include(fornecedor => fornecedor.Produtos)
                .ThenInclude(vinculo => vinculo.ProdutoServico)
            .FirstOrDefaultAsync(fornecedor => fornecedor.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<Fornecedor>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default)
    {
        var consulta = Conjunto.AsNoTracking();

        // Sem ordenação pedida, a listagem sai por nome fantasia: ordem estável entre páginas.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta.OrderBy(fornecedor => fornecedor.NomeFantasia)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }
}
