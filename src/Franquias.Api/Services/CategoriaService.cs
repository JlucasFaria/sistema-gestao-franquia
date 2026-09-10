using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Categorias;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="ICategoriaService"/>.
/// </summary>
public sealed class CategoriaService(
    IRepositorio<Categoria> categorias,
    IRepositorio<ProdutoServico> produtos) : ICategoriaService
{
    /// <inheritdoc />
    public async Task<PagedResult<CategoriaResponse>> ListarAsync(
        QueryParams parametros,
        bool? apenasAtivas,
        CancellationToken cancellationToken = default)
    {
        var consulta = categorias.Consultar();

        if (apenasAtivas is not null)
        {
            consulta = consulta.Where(categoria => categoria.Ativo == apenasAtivas);
        }

        if (!string.IsNullOrWhiteSpace(parametros.Busca))
        {
            var termo = $"%{parametros.Busca.Trim()}%";

            consulta = consulta.Where(categoria => EF.Functions.Like(categoria.Nome, termo));
        }

        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta.OrderBy(categoria => categoria.Nome)
            : consulta.Ordenar(parametros);

        var pagina = await ordenada.PaginarAsync(parametros, cancellationToken);

        return pagina.Converter(CategoriaResponse.De);
    }

    /// <inheritdoc />
    public async Task<CategoriaResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var categoria = await BuscarOuFalharAsync(id, cancellationToken);

        return CategoriaResponse.De(categoria);
    }

    /// <inheritdoc />
    public async Task<CategoriaResponse> CriarAsync(
        CategoriaRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var nome = requisicao.Nome.Trim();

        await ExigirNomeDisponivelAsync(nome, idIgnorado: null, cancellationToken);

        var categoria = new Categoria(nome, requisicao.Descricao);

        await categorias.AdicionarAsync(categoria, cancellationToken);
        await categorias.SalvarAlteracoesAsync(cancellationToken);

        return CategoriaResponse.De(categoria);
    }

    /// <inheritdoc />
    public async Task<CategoriaResponse> AtualizarAsync(
        int id,
        CategoriaRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var categoria = await BuscarOuFalharAsync(id, cancellationToken);
        var nome = requisicao.Nome.Trim();

        await ExigirNomeDisponivelAsync(nome, idIgnorado: id, cancellationToken);

        categoria.Atualizar(nome, requisicao.Descricao);
        await categorias.SalvarAlteracoesAsync(cancellationToken);

        return CategoriaResponse.De(categoria);
    }

    /// <inheritdoc />
    public async Task<CategoriaResponse> AtivarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var categoria = await BuscarOuFalharAsync(id, cancellationToken);

        categoria.Ativar();
        await categorias.SalvarAlteracoesAsync(cancellationToken);

        return CategoriaResponse.De(categoria);
    }

    /// <inheritdoc />
    public async Task<CategoriaResponse> InativarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var categoria = await BuscarOuFalharAsync(id, cancellationToken);

        categoria.Inativar();
        await categorias.SalvarAlteracoesAsync(cancellationToken);

        return CategoriaResponse.De(categoria);
    }

    /// <inheritdoc />
    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoria = await BuscarOuFalharAsync(id, cancellationToken);

        // A chave estrangeira usa Restrict: sem esta checagem, o banco recusaria a exclusão
        // e o cliente receberia um 500 em vez de uma explicação do que fazer.
        if (await produtos.ExisteAsync(produto => produto.CategoriaId == id, cancellationToken))
        {
            throw new RegraDeNegocioException(
                $"A categoria '{categoria.Nome}' possui itens vinculados e não pode ser excluída. "
                + "Inative-a para impedir novos cadastros nela.");
        }

        categorias.Remover(categoria);
        await categorias.SalvarAlteracoesAsync(cancellationToken);
    }

    private async Task<Categoria> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await categorias.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Categoria", id);

    /// <summary>
    /// Recusa nome já usado por outra categoria, sem diferenciar maiúsculas de minúsculas.
    /// O índice único do banco compara byte a byte e deixaria "Bebidas" e "bebidas"
    /// coexistirem; a checagem aqui fecha essa brecha.
    /// </summary>
    private async Task ExigirNomeDisponivelAsync(
        string nome,
        int? idIgnorado,
        CancellationToken cancellationToken)
    {
        var normalizado = nome.ToLowerInvariant();

        var emUso = await categorias.ExisteAsync(
            categoria => categoria.Nome.ToLower() == normalizado
                && (idIgnorado == null || categoria.Id != idIgnorado),
            cancellationToken);

        if (emUso)
        {
            throw new ConflitoException("categoria", "nome", nome);
        }
    }
}
