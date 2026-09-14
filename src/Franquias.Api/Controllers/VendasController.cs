using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Vendas;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Registro, confirmação, cancelamento e consulta das vendas das unidades franqueadas.
/// </summary>
[ApiController]
[Route("api/vendas")]
[Produces("application/json")]
public class VendasController(IVendaService vendas) : ControllerBase
{
    /// <summary>
    /// Lista as vendas de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho e ordenação.</param>
    /// <param name="filtro">
    /// Filtros por unidade, intervalo de datas (inclusivo nas duas pontas, em UTC) e estágio da venda.
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de vendas.</response>
    /// <response code="400">Intervalo de datas invertido ou filtro inválido.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<VendaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<VendaResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] FiltroVendasRequest filtro,
        CancellationToken cancellationToken)
    {
        var pagina = await vendas.ListarAsync(parametros, filtro, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca uma venda pelo identificador, com seus itens.
    /// </summary>
    /// <param name="id">Identificador da venda.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Venda encontrada.</response>
    /// <response code="404">Venda inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(VendaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var venda = await vendas.ObterPorIdAsync(id, cancellationToken);

        return Ok(venda);
    }

    /// <summary>
    /// Registra uma venda pendente. O valor total é calculado a partir dos itens; um total
    /// enviado no corpo da requisição é ignorado. O estoque só é baixado na confirmação.
    /// </summary>
    /// <param name="requisicao">Unidade e itens vendidos.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Venda registrada como pendente.</response>
    /// <response code="400">Venda sem itens ou dados inválidos.</response>
    /// <response code="404">Unidade ou item inexistente.</response>
    /// <response code="422">Unidade que não pode operar ou item indisponível para venda.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(VendaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<VendaResponse>> Registrar(
        [FromBody] CriarVendaRequest requisicao,
        CancellationToken cancellationToken)
    {
        var venda = await vendas.RegistrarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
    }

    /// <summary>
    /// Confirma uma venda pendente e baixa o estoque dos itens físicos na mesma transação.
    /// Se faltar saldo de qualquer item, nada é baixado e a venda continua pendente.
    /// </summary>
    /// <param name="id">Identificador da venda.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Venda confirmada e estoque baixado.</response>
    /// <response code="404">Venda inexistente.</response>
    /// <response code="422">Venda que não está pendente, unidade que não pode operar ou estoque insuficiente.</response>
    [HttpPost("{id:int}/confirmar")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(VendaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<VendaResponse>> Confirmar(int id, CancellationToken cancellationToken)
    {
        var venda = await vendas.ConfirmarAsync(id, cancellationToken);

        return Ok(venda);
    }

    /// <summary>
    /// Cancela uma venda. Se estava confirmada, o estoque é estornado na mesma transação.
    /// Exige perfil de gestão, pois desfaz uma venda já concluída.
    /// </summary>
    /// <param name="id">Identificador da venda.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Venda cancelada.</response>
    /// <response code="403">Perfil sem permissão para cancelar vendas.</response>
    /// <response code="404">Venda inexistente.</response>
    /// <response code="422">Venda já cancelada.</response>
    [HttpPost("{id:int}/cancelar")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(VendaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<VendaResponse>> Cancelar(int id, CancellationToken cancellationToken)
    {
        var venda = await vendas.CancelarAsync(id, cancellationToken);

        return Ok(venda);
    }
}
