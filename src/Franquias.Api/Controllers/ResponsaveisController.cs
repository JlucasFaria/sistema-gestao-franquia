using Franquias.Api.Common.Autenticacao;
using Franquias.Api.DTOs.Responsaveis;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Responsáveis vinculados a uma unidade franqueada, tratados como sub-recurso da unidade.
/// </summary>
[ApiController]
[Route("api/unidades/{unidadeId:int}/responsaveis")]
[Produces("application/json")]
public class ResponsaveisController(IResponsavelService responsaveis) : ControllerBase
{
    /// <summary>
    /// Lista os responsáveis de uma unidade.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Responsáveis da unidade.</response>
    /// <response code="404">Unidade inexistente.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(IReadOnlyCollection<ResponsavelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<ResponsavelResponse>>> Listar(
        int unidadeId,
        CancellationToken cancellationToken)
    {
        var lista = await responsaveis.ListarAsync(unidadeId, cancellationToken);

        return Ok(lista);
    }

    /// <summary>
    /// Vincula um responsável à unidade.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="requisicao">Dados do responsável.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Responsável vinculado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Unidade inexistente.</response>
    /// <response code="409">O CPF informado já responde por esta unidade.</response>
    /// <response code="422">Unidade encerrada.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(ResponsavelResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ResponsavelResponse>> Adicionar(
        int unidadeId,
        [FromBody] CriarResponsavelRequest requisicao,
        CancellationToken cancellationToken)
    {
        var responsavel = await responsaveis.AdicionarAsync(unidadeId, requisicao, cancellationToken);

        return CreatedAtAction(nameof(Listar), new { unidadeId }, responsavel);
    }

    /// <summary>
    /// Altera cargo e dados de contato de um responsável da unidade.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="responsavelId">Identificador do responsável.</param>
    /// <param name="requisicao">Novos dados do responsável.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Responsável atualizado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Unidade inexistente ou responsável que não pertence a ela.</response>
    [HttpPut("{responsavelId:int}")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(ResponsavelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponsavelResponse>> Atualizar(
        int unidadeId,
        int responsavelId,
        [FromBody] AtualizarResponsavelRequest requisicao,
        CancellationToken cancellationToken)
    {
        var responsavel = await responsaveis.AtualizarAsync(unidadeId, responsavelId, requisicao, cancellationToken);

        return Ok(responsavel);
    }

    /// <summary>
    /// Desvincula um responsável da unidade.
    /// </summary>
    /// <param name="unidadeId">Identificador da unidade.</param>
    /// <param name="responsavelId">Identificador do responsável.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Responsável desvinculado.</response>
    /// <response code="404">Unidade inexistente ou responsável que não pertence a ela.</response>
    [HttpDelete("{responsavelId:int}")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(
        int unidadeId,
        int responsavelId,
        CancellationToken cancellationToken)
    {
        await responsaveis.RemoverAsync(unidadeId, responsavelId, cancellationToken);

        return NoContent();
    }
}
