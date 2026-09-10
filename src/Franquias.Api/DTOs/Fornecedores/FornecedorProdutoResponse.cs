using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Fornecedores;

/// <summary>
/// Homologação de um fornecedor para um item do catálogo, com as condições negociadas.
/// </summary>
/// <param name="Id">Identificador do vínculo.</param>
/// <param name="FornecedorId">Fornecedor homologado.</param>
/// <param name="Fornecedor">Nome fantasia do fornecedor.</param>
/// <param name="ProdutoServicoId">Item do catálogo fornecido.</param>
/// <param name="Produto">Nome do item.</param>
/// <param name="EhServico">Indica que o item fornecido é um serviço.</param>
/// <param name="PrecoFornecimento">Preço cobrado pelo fornecedor por unidade do item.</param>
/// <param name="PrazoEntregaEmDias">Prazo de entrega acordado, em dias corridos.</param>
/// <param name="DataCriacao">Momento da homologação, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última renegociação, em UTC.</param>
public sealed record FornecedorProdutoResponse(
    int Id,
    int FornecedorId,
    string Fornecedor,
    int ProdutoServicoId,
    string Produto,
    bool EhServico,
    decimal PrecoFornecimento,
    int PrazoEntregaEmDias,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>
    /// Projeta o vínculo no DTO de saída. Espera fornecedor e item do catálogo carregados.
    /// </summary>
    public static FornecedorProdutoResponse De(FornecedorProduto vinculo) => new(
        vinculo.Id,
        vinculo.FornecedorId,
        vinculo.Fornecedor?.NomeFantasia ?? string.Empty,
        vinculo.ProdutoServicoId,
        vinculo.ProdutoServico?.Nome ?? string.Empty,
        vinculo.ProdutoServico?.EhServico ?? false,
        vinculo.PrecoFornecimento,
        vinculo.PrazoEntregaEmDias,
        vinculo.DataCriacao,
        vinculo.DataAtualizacao);
}
