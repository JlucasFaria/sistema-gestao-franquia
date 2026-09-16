using Franquias.Api.DTOs.Relatorios;

namespace Franquias.Api.Repositories;

/// <summary>
/// Consultas de apoio aos relatórios gerenciais. Diferente dos demais repositórios, este
/// não serve a uma entidade: ele cruza vendas, royalties, estoque e chamados para produzir
/// os indicadores da rede.
/// </summary>
public interface IRelatorioRepositorio
{
    /// <summary>
    /// Totaliza, no banco, o faturamento confirmado de cada unidade no período. Unidades sem
    /// venda confirmada não aparecem no resultado.
    /// </summary>
    /// <param name="dataInicial">Primeiro dia considerado, inclusive; nulo não limita o início.</param>
    /// <param name="dataFinal">Último dia considerado, inclusive; nulo não limita o fim.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<IReadOnlyList<FaturamentoUnidadeResponse>> FaturamentoPorUnidadeAsync(
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Totaliza, no banco, a quantidade vendida e a receita de cada produto ou serviço no
    /// período, considerando apenas vendas confirmadas. A ordenação e o corte dos primeiros
    /// colocados ficam com quem chama.
    /// </summary>
    /// <param name="dataInicial">Primeiro dia considerado, inclusive; nulo não limita o início.</param>
    /// <param name="dataFinal">Último dia considerado, inclusive; nulo não limita o fim.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<IReadOnlyList<ProdutoMaisVendidoResponse>> ProdutosMaisVendidosAsync(
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken = default);
}
