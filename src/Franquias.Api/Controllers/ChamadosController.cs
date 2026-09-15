using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Chamados;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Abertura, acompanhamento e encerramento dos chamados de suporte das unidades. Abrir e
/// responder são atos do dia a dia da unidade; reclassificar e mudar o estágio do
/// atendimento cabem à gestão.
/// </summary>
[ApiController]
[Route("api/chamados")]
[Produces("application/json")]
public class ChamadosController(IChamadoService chamados) : ControllerBase
{
    /// <summary>
    /// Lista os chamados de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho e ordenação.</param>
    /// <param name="filtro">Filtros por unidade, categoria, prioridade, status e situação de abertura.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de chamados.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<ChamadoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ChamadoResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] FiltroChamadosRequest filtro,
        CancellationToken cancellationToken)
    {
        var pagina = await chamados.ListarAsync(parametros, filtro, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca um chamado pelo identificador, com a linha do tempo do atendimento.
    /// </summary>
    /// <param name="id">Identificador do chamado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Chamado encontrado.</response>
    /// <response code="404">Chamado inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(ChamadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChamadoResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var chamado = await chamados.ObterPorIdAsync(id, cancellationToken);

        return Ok(chamado);
    }

    /// <summary>
    /// Abre um chamado de suporte em nome de uma unidade. O autor é o usuário autenticado.
    /// </summary>
    /// <param name="requisicao">Unidade, classificação e relato do problema.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Chamado aberto.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="404">Unidade inexistente.</response>
    /// <response code="422">Unidade inativa.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(ChamadoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ChamadoResponse>> Abrir(
        [FromBody] AbrirChamadoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var chamado = await chamados.AbrirAsync(requisicao, User.ObterIdDoUsuario(), cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, chamado);
    }

    /// <summary>
    /// Acrescenta uma mensagem à linha do tempo de um chamado em aberto.
    /// </summary>
    /// <param name="id">Identificador do chamado.</param>
    /// <param name="requisicao">Conteúdo da mensagem.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Mensagem registrada.</response>
    /// <response code="400">Mensagem inválida.</response>
    /// <response code="404">Chamado inexistente.</response>
    /// <response code="422">Chamado encerrado.</response>
    [HttpPost("{id:int}/interacoes")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(ChamadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ChamadoResponse>> RegistrarInteracao(
        int id,
        [FromBody] RegistrarInteracaoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var chamado = await chamados.RegistrarInteracaoAsync(
            id,
            requisicao,
            User.ObterIdDoUsuario(),
            cancellationToken);

        return Ok(chamado);
    }

    /// <summary>
    /// Reclassifica a urgência de um chamado em aberto.
    /// </summary>
    /// <param name="id">Identificador do chamado.</param>
    /// <param name="requisicao">Nova prioridade.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Prioridade atualizada.</response>
    /// <response code="400">Prioridade inválida.</response>
    /// <response code="404">Chamado inexistente.</response>
    /// <response code="422">Chamado encerrado.</response>
    [HttpPatch("{id:int}/prioridade")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(ChamadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ChamadoResponse>> AlterarPrioridade(
        int id,
        [FromBody] AlterarPrioridadeChamadoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var chamado = await chamados.AlterarPrioridadeAsync(id, requisicao, cancellationToken);

        return Ok(chamado);
    }

    /// <summary>
    /// Avança o chamado para outro estágio de atendimento. O status Encerrado carimba a data
    /// de encerramento e impede novas alterações.
    /// </summary>
    /// <param name="id">Identificador do chamado.</param>
    /// <param name="requisicao">Novo status e observação opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Status atualizado.</response>
    /// <response code="400">Status inválido.</response>
    /// <response code="404">Chamado inexistente.</response>
    /// <response code="422">Chamado encerrado.</response>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(ChamadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ChamadoResponse>> AlterarStatus(
        int id,
        [FromBody] AlterarStatusChamadoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var chamado = await chamados.AlterarStatusAsync(
            id,
            requisicao,
            User.ObterIdDoUsuario(),
            cancellationToken);

        return Ok(chamado);
    }
}
