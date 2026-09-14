using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Geração, consulta e baixa de pagamento das cobranças de royalty. A consulta é restrita à
/// gestão; gerar e dar baixa são atos financeiros do administrador da rede.
/// </summary>
[ApiController]
[Route("api/royalties")]
[Produces("application/json")]
public class RoyaltiesController(IRoyaltyService royalties) : ControllerBase
{
    /// <summary>
    /// Lista as cobranças de royalty de forma paginada. As cobranças vencidas são marcadas
    /// como atrasadas antes da consulta, para que nenhuma apareça como pendente fora do prazo.
    /// </summary>
    /// <param name="parametros">Página, tamanho e ordenação.</param>
    /// <param name="filtro">Filtros por unidade e por situação de pagamento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de cobranças.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(PagedResult<RoyaltyResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<RoyaltyResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] FiltroRoyaltiesRequest filtro,
        CancellationToken cancellationToken)
    {
        var pagina = await royalties.ListarAsync(parametros, filtro, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca uma cobrança de royalty pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da cobrança.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Cobrança encontrada.</response>
    /// <response code="404">Cobrança inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(RoyaltyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoyaltyResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var royalty = await royalties.ObterPorIdAsync(id, cancellationToken);

        return Ok(royalty);
    }

    /// <summary>
    /// Gera a cobrança de royalty de uma unidade em um período já encerrado. O valor é
    /// calculado sobre o faturamento das vendas confirmadas no período e o percentual vigente.
    /// </summary>
    /// <param name="requisicao">Unidade, período de apuração e vencimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Cobrança gerada.</response>
    /// <response code="400">Datas inválidas ou incoerentes.</response>
    /// <response code="404">Unidade inexistente.</response>
    /// <response code="409">Já existe cobrança com período sobreposto.</response>
    /// <response code="422">Período ainda não encerrado ou nada a cobrar.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(RoyaltyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RoyaltyResponse>> Gerar(
        [FromBody] GerarRoyaltyRequest requisicao,
        CancellationToken cancellationToken)
    {
        var royalty = await royalties.GerarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = royalty.Id }, royalty);
    }

    /// <summary>
    /// Dá baixa no pagamento de uma cobrança pendente ou atrasada.
    /// </summary>
    /// <param name="id">Identificador da cobrança.</param>
    /// <param name="requisicao">Data e valor do pagamento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Cobrança quitada.</response>
    /// <response code="400">Dados do pagamento inválidos.</response>
    /// <response code="404">Cobrança inexistente.</response>
    /// <response code="422">Cobrança já quitada, pagamento parcial ou data futura.</response>
    [HttpPost("{id:int}/pagamento")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(RoyaltyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RoyaltyResponse>> RegistrarPagamento(
        int id,
        [FromBody] RegistrarPagamentoRequest requisicao,
        CancellationToken cancellationToken)
    {
        var royalty = await royalties.RegistrarPagamentoAsync(id, requisicao, cancellationToken);

        return Ok(royalty);
    }

    /// <summary>
    /// Marca como atrasadas as cobranças pendentes já vencidas, tomando a data de hoje como
    /// referência. Pode ser chamada a qualquer momento: repetir não produz efeito adicional.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Quantidade de cobranças que passaram a atrasadas.</response>
    [HttpPost("atualizar-atrasos")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(AtualizacaoAtrasosResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AtualizacaoAtrasosResponse>> AtualizarAtrasos(CancellationToken cancellationToken)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var quantidade = await royalties.AtualizarAtrasosAsync(hoje, cancellationToken);

        return Ok(new AtualizacaoAtrasosResponse(hoje, quantidade));
    }
}
