using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Franqueadoras;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e consulta da franqueadora, a empresa dona da marca e da rede.
/// </summary>
[ApiController]
[Route("api/franqueadoras")]
[Produces("application/json")]
public class FranqueadorasController(IFranqueadoraService franqueadoras) : ControllerBase
{
    /// <summary>
    /// Lista as franqueadoras de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho e ordenação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de franqueadoras.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<FranqueadoraResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FranqueadoraResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        CancellationToken cancellationToken)
    {
        var pagina = await franqueadoras.ListarAsync(parametros, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca uma franqueadora pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da franqueadora.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Franqueadora encontrada.</response>
    /// <response code="404">Franqueadora inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(FranqueadoraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranqueadoraResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var franqueadora = await franqueadoras.ObterPorIdAsync(id, cancellationToken);

        return Ok(franqueadora);
    }

    /// <summary>
    /// Cadastra uma franqueadora.
    /// </summary>
    /// <param name="requisicao">Dados cadastrais da franqueadora.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Franqueadora cadastrada.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="409">Já existe franqueadora com o CNPJ informado.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FranqueadoraResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FranqueadoraResponse>> Criar(
        [FromBody] CriarFranqueadoraRequest requisicao,
        CancellationToken cancellationToken)
    {
        var franqueadora = await franqueadoras.CriarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = franqueadora.Id }, franqueadora);
    }

    /// <summary>
    /// Altera os dados cadastrais de uma franqueadora. O CNPJ não é alterável.
    /// </summary>
    /// <param name="id">Identificador da franqueadora.</param>
    /// <param name="requisicao">Novos dados cadastrais.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Franqueadora atualizada.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Franqueadora inexistente.</response>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FranqueadoraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranqueadoraResponse>> Atualizar(
        int id,
        [FromBody] AtualizarFranqueadoraRequest requisicao,
        CancellationToken cancellationToken)
    {
        var franqueadora = await franqueadoras.AtualizarAsync(id, requisicao, cancellationToken);

        return Ok(franqueadora);
    }
}
