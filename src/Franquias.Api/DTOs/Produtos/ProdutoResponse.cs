using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Produtos;

/// <summary>
/// Item do catálogo — produto ou serviço — devolvido pela API.
/// </summary>
/// <param name="Id">Identificador do item.</param>
/// <param name="CategoriaId">Categoria do item.</param>
/// <param name="Categoria">Nome da categoria.</param>
/// <param name="Nome">Nome comercial.</param>
/// <param name="Descricao">Descrição detalhada.</param>
/// <param name="PrecoBase">Preço de tabela sugerido pela franqueadora.</param>
/// <param name="Status">Situação do item no catálogo.</param>
/// <param name="EhServico">Indica que o item é um serviço.</param>
/// <param name="ControlaEstoque">Indica se o item movimenta estoque. Serviços não movimentam.</param>
/// <param name="DisponivelParaVenda">Indica se o item pode ser vendido pelas unidades agora.</param>
/// <param name="Ativo">Indica se o cadastro está ativo.</param>
/// <param name="DataCriacao">Momento do cadastro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
public sealed record ProdutoResponse(
    int Id,
    int CategoriaId,
    string Categoria,
    string Nome,
    string? Descricao,
    decimal PrecoBase,
    StatusProduto Status,
    bool EhServico,
    bool ControlaEstoque,
    bool DisponivelParaVenda,
    bool Ativo,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>Projeta a entidade no DTO de saída. Espera a categoria carregada.</summary>
    public static ProdutoResponse De(ProdutoServico produto) => new(
        produto.Id,
        produto.CategoriaId,
        produto.Categoria?.Nome ?? string.Empty,
        produto.Nome,
        produto.Descricao,
        produto.PrecoBase,
        produto.Status,
        produto.EhServico,
        produto.ControlaEstoque(),
        produto.EstaDisponivelParaVenda(),
        produto.Ativo,
        produto.DataCriacao,
        produto.DataAtualizacao);
}
