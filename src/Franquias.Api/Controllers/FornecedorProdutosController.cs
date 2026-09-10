using Franquias.Api.Common.Autenticacao;
using Franquias.Api.DTOs.Fornecedores;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Produtos e serviços homologados para um fornecedor, tratados como sub-recurso dele.
/// Cada item é identificado na rota pelo seu identificador no catálogo.
/// </summary>
[ApiController]
[Route("api/fornecedores/{fornecedorId:int}/produtos")]
[Produces("application/json")]
public class FornecedorProdutosController(IFornecedorProdutoService homologacoes) : ControllerBase
{
    /// <summary>
    /// Lista os itens homologados para o fornecedor, com as condições negociadas.
    /// </summary>
    /// <param name="fornecedorId">Identificador do fornecedor.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Itens homologados.</response>
    /// <response code="404">Fornecedor inexistente.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(IReadOnlyCollection<FornecedorProdutoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<FornecedorProdutoResponse>>> Listar(
        int fornecedorId,
        CancellationToken cancellationToken)
    {
        var lista = await homologacoes.ListarAsync(fornecedorId, cancellationToken);

        return Ok(lista);
    }

    /// <summary>
    /// Homologa o fornecedor para um produto ou serviço do catálogo.
    /// </summary>
    /// <param name="fornecedorId">Identificador do fornecedor.</param>
    /// <param name="requisicao">Item, preço de fornecimento e prazo de entrega.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Fornecedor homologado para o item.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Fornecedor ou item inexistente.</response>
    /// <response code="409">O fornecedor já está homologado para o item.</response>
    /// <response code="422">Fornecedor inativo, ou item descontinuado ou inativo.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FornecedorProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<FornecedorProdutoResponse>> Associar(
        int fornecedorId,
        [FromBody] AssociarProdutoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var vinculo = await homologacoes.AssociarAsync(fornecedorId, requisicao, cancellationToken);

        return CreatedAtAction(nameof(Listar), new { fornecedorId }, vinculo);
    }

    /// <summary>
    /// Renegocia preço e prazo de um item homologado para o fornecedor.
    /// </summary>
    /// <param name="fornecedorId">Identificador do fornecedor.</param>
    /// <param name="produtoServicoId">Identificador do item no catálogo.</param>
    /// <param name="requisicao">Novas condições comerciais.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Condições atualizadas.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Fornecedor inexistente ou item não homologado para ele.</response>
    [HttpPut("{produtoServicoId:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(FornecedorProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorProdutoResponse>> AtualizarCondicoes(
        int fornecedorId,
        int produtoServicoId,
        [FromBody] AtualizarCondicoesRequest requisicao,
        CancellationToken cancellationToken)
    {
        var vinculo = await homologacoes.AtualizarCondicoesAsync(
            fornecedorId,
            produtoServicoId,
            requisicao,
            cancellationToken);

        return Ok(vinculo);
    }

    /// <summary>
    /// Remove a homologação do fornecedor para um item.
    /// </summary>
    /// <param name="fornecedorId">Identificador do fornecedor.</param>
    /// <param name="produtoServicoId">Identificador do item no catálogo.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Homologação removida.</response>
    /// <response code="404">Fornecedor inexistente ou item não homologado para ele.</response>
    [HttpDelete("{produtoServicoId:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desassociar(
        int fornecedorId,
        int produtoServicoId,
        CancellationToken cancellationToken)
    {
        await homologacoes.DesassociarAsync(fornecedorId, produtoServicoId, cancellationToken);

        return NoContent();
    }
}
