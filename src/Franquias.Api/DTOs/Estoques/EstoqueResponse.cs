using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Estoques;

/// <summary>
/// Saldo de um item do catálogo em uma unidade franqueada.
/// </summary>
/// <param name="Id">Identificador do registro de estoque.</param>
/// <param name="UnidadeFranqueadaId">Unidade dona do saldo.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="ProdutoServicoId">Item do catálogo controlado.</param>
/// <param name="Produto">Nome do item.</param>
/// <param name="Quantidade">Saldo disponível.</param>
/// <param name="QuantidadeMinima">Saldo abaixo do qual o item precisa ser reposto.</param>
/// <param name="AbaixoDoMinimo">Indica que o saldo atingiu o ponto de reposição.</param>
/// <param name="Ativo">Indica se o registro de estoque está ativo.</param>
/// <param name="DataCriacao">Momento em que o controle do item foi aberto, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última movimentação, em UTC.</param>
public sealed record EstoqueResponse(
    int Id,
    int UnidadeFranqueadaId,
    string Unidade,
    int ProdutoServicoId,
    string Produto,
    int Quantidade,
    int QuantidadeMinima,
    bool AbaixoDoMinimo,
    bool Ativo,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>
    /// Projeta a entidade no DTO de saída. Espera unidade e item do catálogo carregados.
    /// </summary>
    public static EstoqueResponse De(Estoque estoque) => new(
        estoque.Id,
        estoque.UnidadeFranqueadaId,
        estoque.UnidadeFranqueada?.NomeFantasia ?? string.Empty,
        estoque.ProdutoServicoId,
        estoque.ProdutoServico?.Nome ?? string.Empty,
        estoque.Quantidade,
        estoque.QuantidadeMinima,
        estoque.EstaAbaixoDoMinimo(),
        estoque.Ativo,
        estoque.DataCriacao,
        estoque.DataAtualizacao);
}
