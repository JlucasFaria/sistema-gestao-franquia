namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Produto ou serviço entre os mais vendidos da rede no período.
/// </summary>
/// <param name="Posicao">Colocação, a partir de 1.</param>
/// <param name="ProdutoServicoId">Identificador do produto ou serviço.</param>
/// <param name="Produto">Nome do produto ou serviço.</param>
/// <param name="Categoria">Categoria a que pertence.</param>
/// <param name="EhServico">Indica se é um serviço, e não um produto físico.</param>
/// <param name="QuantidadeVendida">Total de unidades vendidas no período.</param>
/// <param name="ValorTotal">Receita gerada pelo item no período.</param>
public sealed record ProdutoMaisVendidoResponse(
    int Posicao,
    int ProdutoServicoId,
    string Produto,
    string Categoria,
    bool EhServico,
    int QuantidadeVendida,
    decimal ValorTotal);
