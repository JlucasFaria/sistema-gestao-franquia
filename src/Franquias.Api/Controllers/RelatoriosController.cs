using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Autenticacao;
using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Relatórios e indicadores gerenciais da rede. Todos aceitam recorte por período, com as
/// duas pontas inclusivas; sem datas, cobrem toda a história da rede.
/// </summary>
[ApiController]
[Route("api/relatorios")]
[Produces("application/json")]
public class RelatoriosController(IRelatorioService relatorios) : ControllerBase
{
    /// <summary>
    /// Faturamento confirmado da rede no período, aberto por unidade, da maior para a menor.
    /// </summary>
    /// <param name="filtro">Intervalo de datas considerado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Faturamento do período.</response>
    /// <response code="400">Intervalo de datas invertido.</response>
    [HttpGet("faturamento")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(RelatorioFaturamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RelatorioFaturamentoResponse>> Faturamento(
        [FromQuery] FiltroPeriodoRequest filtro,
        CancellationToken cancellationToken)
    {
        var relatorio = await relatorios.FaturamentoPorUnidadeAsync(filtro, cancellationToken);

        return Ok(relatorio);
    }

    /// <summary>
    /// Ranking das unidades por faturamento no período, com a participação de cada uma no
    /// total da rede.
    /// </summary>
    /// <param name="filtro">Intervalo de datas considerado.</param>
    /// <param name="limite">Quantas colocações devolver; sem o parâmetro, devolve todas.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Ranking do período.</response>
    /// <response code="400">Intervalo de datas invertido ou limite fora da faixa.</response>
    [HttpGet("ranking-unidades")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(IReadOnlyList<RankingUnidadeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<RankingUnidadeResponse>>> RankingDeUnidades(
        [FromQuery] FiltroPeriodoRequest filtro,
        [FromQuery][Range(1, 100, ErrorMessage = "O limite deve estar entre 1 e 100.")] int? limite,
        CancellationToken cancellationToken)
    {
        var ranking = await relatorios.RankingPorFaturamentoAsync(filtro, limite, cancellationToken);

        return Ok(ranking);
    }

    /// <summary>
    /// Royalties gerados no período, com o total da rede e a abertura por unidade.
    /// </summary>
    /// <param name="filtro">Intervalo de competências considerado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Royalties do período.</response>
    /// <response code="400">Intervalo de datas invertido.</response>
    [HttpGet("royalties")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(RelatorioRoyaltiesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RelatorioRoyaltiesResponse>> Royalties(
        [FromQuery] FiltroPeriodoRequest filtro,
        CancellationToken cancellationToken)
    {
        var relatorio = await relatorios.RoyaltiesGeradosAsync(filtro, cancellationToken);

        return Ok(relatorio);
    }

    /// <summary>
    /// Produtos e serviços mais vendidos no período, da maior para a menor quantidade.
    /// </summary>
    /// <param name="filtro">Intervalo de datas considerado.</param>
    /// <param name="limite">Quantas colocações devolver; sem o parâmetro, devolve todas.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Itens mais vendidos no período.</response>
    /// <response code="400">Intervalo de datas invertido ou limite fora da faixa.</response>
    [HttpGet("produtos-mais-vendidos")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(IReadOnlyList<ProdutoMaisVendidoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ProdutoMaisVendidoResponse>>> ProdutosMaisVendidos(
        [FromQuery] FiltroPeriodoRequest filtro,
        [FromQuery][Range(1, 100, ErrorMessage = "O limite deve estar entre 1 e 100.")] int? limite,
        CancellationToken cancellationToken)
    {
        var itens = await relatorios.ProdutosMaisVendidosAsync(filtro, limite, cancellationToken);

        return Ok(itens);
    }

    /// <summary>
    /// Itens de estoque abaixo da quantidade mínima, dos que faltam mais para os que faltam
    /// menos. É o alerta de reposição da rede.
    /// </summary>
    /// <param name="unidadeFranqueadaId">Restringe a uma unidade; sem o parâmetro, a rede toda.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Itens em estoque crítico.</response>
    [HttpGet("estoque-critico")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(IReadOnlyList<EstoqueCriticoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EstoqueCriticoResponse>>> EstoqueCritico(
        [FromQuery] int? unidadeFranqueadaId,
        CancellationToken cancellationToken)
    {
        var itens = await relatorios.EstoqueCriticoAsync(unidadeFranqueadaId, cancellationToken);

        return Ok(itens);
    }

    /// <summary>
    /// Quantidade de chamados em cada estágio de atendimento, entre os abertos no período.
    /// </summary>
    /// <param name="filtro">Intervalo de datas de abertura considerado.</param>
    /// <param name="unidadeFranqueadaId">Restringe a uma unidade; sem o parâmetro, a rede toda.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Chamados por estágio de atendimento.</response>
    /// <response code="400">Intervalo de datas invertido.</response>
    [HttpGet("chamados-por-status")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(RelatorioChamadosResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RelatorioChamadosResponse>> ChamadosPorStatus(
        [FromQuery] FiltroPeriodoRequest filtro,
        [FromQuery] int? unidadeFranqueadaId,
        CancellationToken cancellationToken)
    {
        var relatorio = await relatorios.ChamadosPorStatusAsync(filtro, unidadeFranqueadaId, cancellationToken);

        return Ok(relatorio);
    }
}
