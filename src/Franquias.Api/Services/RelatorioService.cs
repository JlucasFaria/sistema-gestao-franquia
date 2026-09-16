using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IRelatorioService"/>.
/// </summary>
public sealed class RelatorioService(IRelatorioRepositorio relatorios) : IRelatorioService
{
    /// <inheritdoc />
    public async Task<RelatorioFaturamentoResponse> FaturamentoPorUnidadeAsync(
        FiltroPeriodoRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var unidades = await relatorios.FaturamentoPorUnidadeAsync(
            filtro.DataInicial,
            filtro.DataFinal,
            cancellationToken);

        // A ordenação acontece aqui porque o SQLite não ordena por colunas decimais.
        var ordenadas = unidades
            .OrderByDescending(unidade => unidade.ValorTotal)
            .ThenBy(unidade => unidade.Unidade)
            .ToList();

        var quantidade = ordenadas.Sum(unidade => unidade.QuantidadeDeVendas);
        var total = ordenadas.Sum(unidade => unidade.ValorTotal);

        return new RelatorioFaturamentoResponse(
            filtro.DataInicial,
            filtro.DataFinal,
            quantidade,
            total,
            quantidade == 0 ? decimal.Zero : Math.Round(total / quantidade, 2, MidpointRounding.AwayFromZero),
            ordenadas);
    }
}
