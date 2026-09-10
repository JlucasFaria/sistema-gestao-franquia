using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Fornecedores;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e consulta dos fornecedores homologados pela rede. Como envolve condições
/// comerciais, a leitura é restrita à gestão da rede e das unidades.
/// </summary>
[ApiController]
[Route("api/fornecedores")]
[Produces("application/json")]
public class FornecedoresController(IFornecedorService fornecedores) : ControllerBase
{
    /// <summary>
    /// Lista os fornecedores de forma paginada.
    /// </summary>
    /// <param name="parametros">
    /// Página, tamanho, ordenação e busca por nome fantasia, razão social ou trecho do CNPJ.
    /// </param>
    /// <param name="filtro">Filtros por situação (ativo ou inativo) e por item homologado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de fornecedores.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(PagedResult<FornecedorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FornecedorResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] FiltroFornecedoresRequest filtro,
        CancellationToken cancellationToken)
    {
        var pagina = await fornecedores.ListarAsync(parametros, filtro, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca um fornecedor pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do fornecedor.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Fornecedor encontrado.</response>
    /// <response code="404">Fornecedor inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(FornecedorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var fornecedor = await fornecedores.ObterPorIdAsync(id, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Cadastra um fornecedor.
    /// </summary>
    /// <param name="requisicao">Dados cadastrais do fornecedor.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Fornecedor cadastrado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="409">Já existe fornecedor com o CNPJ informado.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FornecedorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FornecedorResponse>> Criar(
        [FromBody] CriarFornecedorRequest requisicao,
        CancellationToken cancellationToken)
    {
        var fornecedor = await fornecedores.CriarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, fornecedor);
    }

    /// <summary>
    /// Altera os dados cadastrais de um fornecedor. O CNPJ não é alterável.
    /// </summary>
    /// <param name="id">Identificador do fornecedor.</param>
    /// <param name="requisicao">Novos dados cadastrais.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Fornecedor atualizado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Fornecedor inexistente.</response>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FornecedorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorResponse>> Atualizar(
        int id,
        [FromBody] AtualizarFornecedorRequest requisicao,
        CancellationToken cancellationToken)
    {
        var fornecedor = await fornecedores.AtualizarAsync(id, requisicao, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Exclui um fornecedor sem produtos homologados.
    /// </summary>
    /// <param name="id">Identificador do fornecedor.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Fornecedor excluído.</response>
    /// <response code="404">Fornecedor inexistente.</response>
    /// <response code="422">O fornecedor possui produtos homologados; deve ser inativado.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await fornecedores.RemoverAsync(id, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Reativa um fornecedor.
    /// </summary>
    /// <param name="id">Identificador do fornecedor.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Fornecedor ativo. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Fornecedor inexistente.</response>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FornecedorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorResponse>> Ativar(int id, CancellationToken cancellationToken)
    {
        var fornecedor = await fornecedores.AtivarAsync(id, cancellationToken);

        return Ok(fornecedor);
    }

    /// <summary>
    /// Inativa um fornecedor, impedindo novas homologações sem apagar o histórico.
    /// </summary>
    /// <param name="id">Identificador do fornecedor.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Fornecedor inativo. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Fornecedor inexistente.</response>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FornecedorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorResponse>> Inativar(int id, CancellationToken cancellationToken)
    {
        var fornecedor = await fornecedores.InativarAsync(id, cancellationToken);

        return Ok(fornecedor);
    }
}
