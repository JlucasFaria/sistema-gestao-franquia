using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Franqueados;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e consulta dos franqueados. Como os dados incluem CPF, a leitura é restrita à
/// gestão da rede e das unidades.
/// </summary>
[ApiController]
[Route("api/franqueados")]
[Produces("application/json")]
public class FranqueadosController(IFranqueadoService franqueados) : ControllerBase
{
    /// <summary>
    /// Lista os franqueados de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho, ordenação e busca por nome ou CPF.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de franqueados.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(PagedResult<FranqueadoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FranqueadoResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        CancellationToken cancellationToken)
    {
        var pagina = await franqueados.ListarAsync(parametros, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca um franqueado pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do franqueado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Franqueado encontrado.</response>
    /// <response code="404">Franqueado inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(FranqueadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranqueadoResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var franqueado = await franqueados.ObterPorIdAsync(id, cancellationToken);

        return Ok(franqueado);
    }

    /// <summary>
    /// Cadastra um franqueado.
    /// </summary>
    /// <param name="requisicao">Dados do franqueado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Franqueado cadastrado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="409">Já existe franqueado com o CPF informado.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FranqueadoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FranqueadoResponse>> Criar(
        [FromBody] CriarFranqueadoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var franqueado = await franqueados.CriarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = franqueado.Id }, franqueado);
    }

    /// <summary>
    /// Altera os dados de contato de um franqueado. O CPF não é alterável.
    /// </summary>
    /// <param name="id">Identificador do franqueado.</param>
    /// <param name="requisicao">Novos dados de contato.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Franqueado atualizado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Franqueado inexistente.</response>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FranqueadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranqueadoResponse>> Atualizar(
        int id,
        [FromBody] AtualizarFranqueadoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var franqueado = await franqueados.AtualizarAsync(id, requisicao, cancellationToken);

        return Ok(franqueado);
    }
}
