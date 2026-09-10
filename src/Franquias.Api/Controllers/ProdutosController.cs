using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Catálogo de produtos e serviços da rede. Não há exclusão física: itens já vendidos ou
/// com estoque precisam continuar existindo, então o cadastro é inativado.
/// </summary>
[ApiController]
[Route("api/produtos")]
[Produces("application/json")]
public class ProdutosController(IProdutoServicoService produtos) : ControllerBase
{
    /// <summary>
    /// Lista os itens do catálogo de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho e ordenação.</param>
    /// <param name="filtro">Filtros por categoria e natureza (produto ou serviço).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de itens.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PagedResult<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProdutoResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] FiltroProdutosRequest filtro,
        CancellationToken cancellationToken)
    {
        var pagina = await produtos.ListarAsync(parametros, filtro, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca um item do catálogo pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do item.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Item encontrado.</response>
    /// <response code="404">Item inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var produto = await produtos.ObterPorIdAsync(id, cancellationToken);

        return Ok(produto);
    }

    /// <summary>
    /// Cadastra um produto ou serviço. O item nasce com status Ativo.
    /// </summary>
    /// <param name="requisicao">Dados do item.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Item cadastrado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Categoria inexistente.</response>
    /// <response code="422">Categoria inativa.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ProdutoResponse>> Criar(
        [FromBody] CriarProdutoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var produto = await produtos.CriarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    /// <summary>
    /// Altera categoria, nome, descrição e preço base de um item. A natureza — produto ou
    /// serviço — não é alterável.
    /// </summary>
    /// <param name="id">Identificador do item.</param>
    /// <param name="requisicao">Novos dados de catálogo.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Item atualizado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Item ou categoria inexistente.</response>
    /// <response code="422">Tentativa de mover o item para uma categoria inativa.</response>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ProdutoResponse>> Atualizar(
        int id,
        [FromBody] AtualizarProdutoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var produto = await produtos.AtualizarAsync(id, requisicao, cancellationToken);

        return Ok(produto);
    }

    /// <summary>
    /// Altera a situação do item no catálogo. Item descontinuado não volta ao catálogo.
    /// </summary>
    /// <param name="id">Identificador do item.</param>
    /// <param name="requisicao">Nova situação.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Situação aplicada.</response>
    /// <response code="400">Status inválido.</response>
    /// <response code="404">Item inexistente.</response>
    /// <response code="422">Item descontinuado.</response>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ProdutoResponse>> AlterarStatus(
        int id,
        [FromBody] AlterarStatusProdutoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var produto = await produtos.AlterarStatusAsync(id, requisicao.Status!.Value, cancellationToken);

        return Ok(produto);
    }

    /// <summary>
    /// Reativa o cadastro de um item.
    /// </summary>
    /// <param name="id">Identificador do item.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Item ativo. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Item inexistente.</response>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponse>> Ativar(int id, CancellationToken cancellationToken)
    {
        var produto = await produtos.AtivarAsync(id, cancellationToken);

        return Ok(produto);
    }

    /// <summary>
    /// Inativa o cadastro de um item, sem removê-lo. Substitui a exclusão.
    /// </summary>
    /// <param name="id">Identificador do item.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Item inativo. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Item inexistente.</response>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponse>> Inativar(int id, CancellationToken cancellationToken)
    {
        var produto = await produtos.InativarAsync(id, cancellationToken);

        return Ok(produto);
    }
}
