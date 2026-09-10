using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Categorias;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e consulta das categorias do catálogo da rede.
/// </summary>
[ApiController]
[Route("api/categorias")]
[Produces("application/json")]
public class CategoriasController(ICategoriaService categorias) : ControllerBase
{
    /// <summary>
    /// Lista as categorias de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho, ordenação e busca por nome.</param>
    /// <param name="apenasAtivas">Filtra por situação. Omitido, traz ativas e inativas.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de categorias.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<CategoriaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CategoriaResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] bool? apenasAtivas,
        CancellationToken cancellationToken)
    {
        var pagina = await categorias.ListarAsync(parametros, apenasAtivas, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Categoria encontrada.</response>
    /// <response code="404">Categoria inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.ObterPorIdAsync(id, cancellationToken);

        return Ok(categoria);
    }

    /// <summary>
    /// Cadastra uma categoria.
    /// </summary>
    /// <param name="requisicao">Nome e descrição da categoria.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Categoria cadastrada.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="409">Já existe categoria com o nome informado.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoriaResponse>> Criar(
        [FromBody] CategoriaRequest requisicao,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.CriarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
    }

    /// <summary>
    /// Altera o nome e a descrição de uma categoria.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="requisicao">Novos nome e descrição.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Categoria atualizada.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Categoria inexistente.</response>
    /// <response code="409">O nome informado já pertence a outra categoria.</response>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoriaResponse>> Atualizar(
        int id,
        [FromBody] CategoriaRequest requisicao,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.AtualizarAsync(id, requisicao, cancellationToken);

        return Ok(categoria);
    }

    /// <summary>
    /// Exclui uma categoria sem itens vinculados.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Categoria excluída.</response>
    /// <response code="404">Categoria inexistente.</response>
    /// <response code="422">A categoria possui itens vinculados; deve ser inativada.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await categorias.RemoverAsync(id, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Reativa uma categoria.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Categoria ativa. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Categoria inexistente.</response>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> Ativar(int id, CancellationToken cancellationToken)
    {
        var categoria = await categorias.AtivarAsync(id, cancellationToken);

        return Ok(categoria);
    }

    /// <summary>
    /// Inativa uma categoria, impedindo novos itens nela sem afetar os existentes.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Categoria inativa. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Categoria inexistente.</response>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> Inativar(int id, CancellationToken cancellationToken)
    {
        var categoria = await categorias.InativarAsync(id, cancellationToken);

        return Ok(categoria);
    }
}
