using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Categorias;

/// <summary>
/// Categoria do catálogo devolvida pela API.
/// </summary>
/// <param name="Id">Identificador da categoria.</param>
/// <param name="Nome">Nome da categoria.</param>
/// <param name="Descricao">Descrição do que a categoria agrupa.</param>
/// <param name="Ativo">Indica se a categoria aceita novos itens.</param>
/// <param name="DataCriacao">Momento do cadastro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
public sealed record CategoriaResponse(
    int Id,
    string Nome,
    string? Descricao,
    bool Ativo,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>Projeta a entidade no DTO de saída.</summary>
    public static CategoriaResponse De(Categoria categoria) => new(
        categoria.Id,
        categoria.Nome,
        categoria.Descricao,
        categoria.Ativo,
        categoria.DataCriacao,
        categoria.DataAtualizacao);
}
