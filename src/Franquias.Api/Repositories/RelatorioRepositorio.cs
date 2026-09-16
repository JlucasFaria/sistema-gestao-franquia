using Franquias.Api.Data;
using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IRelatorioRepositorio"/>.
/// </summary>
public class RelatorioRepositorio(AppDbContext contexto) : IRelatorioRepositorio
{
    /// <inheritdoc />
    /// <remarks>
    /// A soma é feita pelo banco, que trata valores decimais sem perda de precisão. O ticket
    /// médio é calculado aqui, depois de materializar os grupos, porque é uma divisão entre
    /// decimais que o SQLite não faz.
    /// </remarks>
    public async Task<IReadOnlyList<FaturamentoUnidadeResponse>> FaturamentoPorUnidadeAsync(
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken = default)
    {
        var grupos = await VendasConfirmadas(dataInicial, dataFinal)
            .GroupBy(venda => new { venda.UnidadeFranqueadaId, venda.UnidadeFranqueada.NomeFantasia })
            .Select(grupo => new
            {
                grupo.Key.UnidadeFranqueadaId,
                grupo.Key.NomeFantasia,
                Quantidade = grupo.Count(),
                Total = grupo.Sum(venda => venda.ValorTotal)
            })
            .ToListAsync(cancellationToken);

        return [.. grupos.Select(grupo => new FaturamentoUnidadeResponse(
            grupo.UnidadeFranqueadaId,
            grupo.NomeFantasia,
            grupo.Quantidade,
            grupo.Total,
            CalcularTicketMedio(grupo.Total, grupo.Quantidade)))];
    }

    /// <summary>
    /// Valor médio por venda, arredondado a dois centavos. Sem venda no período, o ticket
    /// médio é zero em vez de uma divisão por zero.
    /// </summary>
    protected static decimal CalcularTicketMedio(decimal total, int quantidade) =>
        quantidade == 0
            ? decimal.Zero
            : Math.Round(total / quantidade, 2, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Base de todos os relatórios de faturamento: apenas vendas confirmadas, no intervalo
    /// pedido. A venda pendente ainda pode não se concretizar e a cancelada não é receita.
    /// </summary>
    /// <remarks>
    /// A data final é inclusiva: o limite é o primeiro instante do dia seguinte, com
    /// comparação estrita, para não deixar de fora as vendas do último dia.
    /// </remarks>
    protected IQueryable<Venda> VendasConfirmadas(DateOnly? dataInicial, DateOnly? dataFinal)
    {
        var consulta = contexto.Vendas
            .AsNoTracking()
            .Where(venda => venda.Status == StatusVenda.Confirmada);

        if (dataInicial is not null)
        {
            var inicio = dataInicial.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            consulta = consulta.Where(venda => venda.DataVenda >= inicio);
        }

        if (dataFinal is not null)
        {
            var fimExclusivo = dataFinal.Value.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            consulta = consulta.Where(venda => venda.DataVenda < fimExclusivo);
        }

        return consulta;
    }

    /// <inheritdoc />
    /// <remarks>
    /// A posição sai como zero daqui: só depois de ordenar é que se sabe quem é o primeiro.
    /// </remarks>
    public async Task<IReadOnlyList<ProdutoMaisVendidoResponse>> ProdutosMaisVendidosAsync(
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        CancellationToken cancellationToken = default)
    {
        var vendas = VendasConfirmadas(dataInicial, dataFinal);

        var grupos = await contexto.ItensVenda
            .AsNoTracking()
            .Where(item => vendas.Any(venda => venda.Id == item.VendaId))
            .GroupBy(item => new
            {
                item.ProdutoServicoId,
                item.ProdutoServico.Nome,
                Categoria = item.ProdutoServico.Categoria.Nome,
                item.ProdutoServico.EhServico
            })
            .Select(grupo => new
            {
                grupo.Key.ProdutoServicoId,
                grupo.Key.Nome,
                grupo.Key.Categoria,
                grupo.Key.EhServico,
                Quantidade = grupo.Sum(item => item.Quantidade),
                Total = grupo.Sum(item => item.Subtotal)
            })
            .ToListAsync(cancellationToken);

        return [.. grupos.Select(grupo => new ProdutoMaisVendidoResponse(
            0,
            grupo.ProdutoServicoId,
            grupo.Nome,
            grupo.Categoria,
            grupo.EhServico,
            grupo.Quantidade,
            grupo.Total))];
    }

    /// <inheritdoc />
    /// <remarks>
    /// O critério é o mesmo de <see cref="Estoque.EstaAbaixoDoMinimo"/>, reescrito aqui em
    /// forma de consulta porque o EF não traduz o método da entidade. Itens com mínimo zero
    /// nunca entram, já que o saldo não fica negativo.
    /// </remarks>
    public async Task<IReadOnlyList<EstoqueCriticoResponse>> EstoqueCriticoAsync(
        int? unidadeFranqueadaId,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Estoques
            .AsNoTracking()
            .Where(estoque => estoque.Quantidade < estoque.QuantidadeMinima);

        if (unidadeFranqueadaId is not null)
        {
            consulta = consulta.Where(estoque => estoque.UnidadeFranqueadaId == unidadeFranqueadaId);
        }

        return await consulta
            .OrderByDescending(estoque => estoque.QuantidadeMinima - estoque.Quantidade)
            .ThenBy(estoque => estoque.UnidadeFranqueada.NomeFantasia)
            .ThenBy(estoque => estoque.ProdutoServico.Nome)
            .Select(estoque => new EstoqueCriticoResponse(
                estoque.UnidadeFranqueadaId,
                estoque.UnidadeFranqueada.NomeFantasia,
                estoque.ProdutoServicoId,
                estoque.ProdutoServico.Nome,
                estoque.ProdutoServico.Categoria.Nome,
                estoque.Quantidade,
                estoque.QuantidadeMinima,
                estoque.QuantidadeMinima - estoque.Quantidade))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    /// <remarks>
    /// O corte é pela data de abertura do chamado, que é a data de criação do registro.
    /// </remarks>
    public async Task<IReadOnlyList<ContagemPorStatusResponse>> ChamadosPorStatusAsync(
        DateOnly? dataInicial,
        DateOnly? dataFinal,
        int? unidadeFranqueadaId,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Chamados.AsNoTracking();

        if (unidadeFranqueadaId is not null)
        {
            consulta = consulta.Where(chamado => chamado.UnidadeFranqueadaId == unidadeFranqueadaId);
        }

        if (dataInicial is not null)
        {
            var inicio = dataInicial.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            consulta = consulta.Where(chamado => chamado.DataCriacao >= inicio);
        }

        if (dataFinal is not null)
        {
            var fimExclusivo = dataFinal.Value.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            consulta = consulta.Where(chamado => chamado.DataCriacao < fimExclusivo);
        }

        return await consulta
            .GroupBy(chamado => chamado.Status)
            .Select(grupo => new ContagemPorStatusResponse(grupo.Key, grupo.Count()))
            .ToListAsync(cancellationToken);
    }
}
