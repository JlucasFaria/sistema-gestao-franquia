using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e consulta das unidades franqueadas. Não há exclusão física: unidades são
/// inativadas, preservando o histórico de vendas, estoque e royalties.
/// </summary>
[ApiController]
[Route("api/unidades")]
[Produces("application/json")]
public class UnidadesController(IUnidadeService unidades) : ControllerBase
{
    /// <summary>
    /// Lista as unidades de forma paginada.
    /// </summary>
    /// <param name="parametros">
    /// Página, tamanho, ordenação e busca livre por nome, cidade, CNPJ ou nome de responsável.
    /// </param>
    /// <param name="filtro">
    /// Filtros por situação contratual, cadastro ativo ou inativo, cidade, UF, franqueadora e franqueado.
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de unidades.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<UnidadeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UnidadeResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] FiltroUnidadesRequest filtro,
        CancellationToken cancellationToken)
    {
        var pagina = await unidades.ListarAsync(parametros, filtro, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca uma unidade pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da unidade.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Unidade encontrada.</response>
    /// <response code="404">Unidade inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.ObterPorIdAsync(id, cancellationToken);

        return Ok(unidade);
    }

    /// <summary>
    /// Cadastra uma unidade franqueada. A unidade nasce em implantação e só passa a operar
    /// depois de ter a situação alterada para Ativa.
    /// </summary>
    /// <param name="requisicao">Dados da unidade.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Unidade cadastrada.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Franqueadora ou franqueado inexistente.</response>
    /// <response code="409">Já existe unidade com o CNPJ informado.</response>
    /// <response code="422">Franqueadora ou franqueado inativo.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UnidadeResponse>> Criar(
        [FromBody] CriarUnidadeRequest requisicao,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.CriarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, unidade);
    }

    /// <summary>
    /// Altera os dados cadastrais de uma unidade. CNPJ, franqueadora e franqueado não são alteráveis.
    /// </summary>
    /// <param name="id">Identificador da unidade.</param>
    /// <param name="requisicao">Novos dados cadastrais.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Unidade atualizada.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Unidade inexistente.</response>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeResponse>> Atualizar(
        int id,
        [FromBody] AtualizarUnidadeRequest requisicao,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.AtualizarAsync(id, requisicao, cancellationToken);

        return Ok(unidade);
    }

    /// <summary>
    /// Altera a situação contratual de uma unidade. Unidade encerrada não muda mais de situação.
    /// </summary>
    /// <param name="id">Identificador da unidade.</param>
    /// <param name="requisicao">Nova situação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Situação aplicada.</response>
    /// <response code="400">Situação inválida.</response>
    /// <response code="404">Unidade inexistente.</response>
    /// <response code="422">Unidade encerrada.</response>
    [HttpPatch("{id:int}/situacao")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UnidadeResponse>> AlterarSituacao(
        int id,
        [FromBody] AlterarSituacaoUnidadeRequest requisicao,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.AlterarSituacaoAsync(id, requisicao.Situacao!.Value, cancellationToken);

        return Ok(unidade);
    }

    /// <summary>
    /// Reativa o cadastro de uma unidade.
    /// </summary>
    /// <param name="id">Identificador da unidade.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Unidade ativa. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Unidade inexistente.</response>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeResponse>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.AtivarAsync(id, cancellationToken);

        return Ok(unidade);
    }

    /// <summary>
    /// Inativa o cadastro de uma unidade, sem removê-la. Substitui a exclusão: vendas,
    /// estoque e royalties continuam vinculados à unidade.
    /// </summary>
    /// <param name="id">Identificador da unidade.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Unidade inativa. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Unidade inexistente.</response>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UnidadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeResponse>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.InativarAsync(id, cancellationToken);

        return Ok(unidade);
    }
}
