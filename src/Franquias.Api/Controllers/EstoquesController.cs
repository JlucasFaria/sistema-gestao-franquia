using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Estoques;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Saldos de estoque por unidade franqueada e as movimentações que os alteram.
/// </summary>
[ApiController]
[Route("api/estoques")]
[Produces("application/json")]
public class EstoquesController(IEstoqueService estoques) : ControllerBase
{
    /// <summary>
    /// Lista os saldos de estoque da rede de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho, ordenação e busca por nome do item.</param>
    /// <param name="filtro">Filtros por unidade e por item do catálogo.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de saldos.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<EstoqueResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EstoqueResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] FiltroEstoquesRequest filtro,
        CancellationToken cancellationToken)
    {
        var pagina = await estoques.ListarAsync(parametros, filtro, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Consulta o saldo de um item em uma unidade.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Saldo do item na unidade.</response>
    /// <response code="404">Unidade, item, ou controle de estoque inexistente.</response>
    [HttpGet("unidades/{unidadeId:int}/produtos/{produtoServicoId:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstoqueResponse>> ObterSaldo(
        int unidadeId,
        int produtoServicoId,
        CancellationToken cancellationToken)
    {
        var saldo = await estoques.ObterSaldoAsync(unidadeId, produtoServicoId, cancellationToken);

        return Ok(saldo);
    }

    /// <summary>
    /// Lista o histórico de movimentações de um item na unidade, da mais recente para a mais antiga.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="parametros">Página, tamanho e ordenação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página do histórico de movimentações.</response>
    /// <response code="404">Unidade, item, ou controle de estoque inexistente.</response>
    [HttpGet("unidades/{unidadeId:int}/produtos/{produtoServicoId:int}/movimentacoes")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<MovimentacaoEstoqueResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<MovimentacaoEstoqueResponse>>> ListarMovimentacoes(
        int unidadeId,
        int produtoServicoId,
        [FromQuery] QueryParams parametros,
        CancellationToken cancellationToken)
    {
        var pagina = await estoques.ListarMovimentacoesAsync(
            unidadeId,
            produtoServicoId,
            parametros,
            cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Dá entrada no estoque. A primeira entrada de um item abre o controle dele na unidade.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="requisicao">Quantidade e justificativa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Saldo após a entrada.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Unidade ou item inexistente.</response>
    /// <response code="422">Unidade encerrada, item descontinuado ou serviço.</response>
    [HttpPost("unidades/{unidadeId:int}/produtos/{produtoServicoId:int}/entrada")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EstoqueResponse>> RegistrarEntrada(
        int unidadeId,
        int produtoServicoId,
        [FromBody] MovimentacaoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var saldo = await estoques.RegistrarEntradaAsync(
            unidadeId,
            produtoServicoId,
            requisicao,
            cancellationToken);

        return Ok(saldo);
    }

    /// <summary>
    /// Dá baixa no estoque. A operação é recusada quando o saldo é insuficiente.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="requisicao">Quantidade e justificativa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Saldo após a baixa.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Unidade, item, ou controle de estoque inexistente.</response>
    /// <response code="422">Saldo insuficiente, unidade encerrada ou item que não movimenta estoque.</response>
    [HttpPost("unidades/{unidadeId:int}/produtos/{produtoServicoId:int}/saida")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EstoqueResponse>> RegistrarSaida(
        int unidadeId,
        int produtoServicoId,
        [FromBody] MovimentacaoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var saldo = await estoques.RegistrarSaidaAsync(
            unidadeId,
            produtoServicoId,
            requisicao,
            cancellationToken);

        return Ok(saldo);
    }

    /// <summary>
    /// Corrige o saldo para a quantidade apurada em contagem física. Exige perfil de gestão:
    /// é por este caminho que uma perda poderia ser encoberta.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="requisicao">Quantidade apurada e justificativa.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Saldo após o ajuste.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="403">Perfil sem permissão para ajustar estoque.</response>
    /// <response code="404">Unidade, item, ou controle de estoque inexistente.</response>
    /// <response code="422">Unidade encerrada ou item que não movimenta estoque.</response>
    [HttpPost("unidades/{unidadeId:int}/produtos/{produtoServicoId:int}/ajuste")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EstoqueResponse>> Ajustar(
        int unidadeId,
        int produtoServicoId,
        [FromBody] AjusteEstoqueRequest requisicao,
        CancellationToken cancellationToken)
    {
        var saldo = await estoques.AjustarAsync(
            unidadeId,
            produtoServicoId,
            requisicao,
            cancellationToken);

        return Ok(saldo);
    }

    /// <summary>
    /// Define o ponto de reposição do item na unidade, abrindo o controle com saldo zero se
    /// o item ainda não for controlado ali.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="produtoServicoId">Identificador do item do catálogo.</param>
    /// <param name="requisicao">Quantidade mínima.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Saldo com o novo ponto de reposição.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="403">Perfil sem permissão para definir o ponto de reposição.</response>
    /// <response code="404">Unidade ou item inexistente.</response>
    /// <response code="422">Unidade encerrada ou item que não movimenta estoque.</response>
    [HttpPut("unidades/{unidadeId:int}/produtos/{produtoServicoId:int}/minimo")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EstoqueResponse>> DefinirQuantidadeMinima(
        int unidadeId,
        int produtoServicoId,
        [FromBody] DefinirQuantidadeMinimaRequest requisicao,
        CancellationToken cancellationToken)
    {
        var saldo = await estoques.DefinirQuantidadeMinimaAsync(
            unidadeId,
            produtoServicoId,
            requisicao,
            cancellationToken);

        return Ok(saldo);
    }
}
