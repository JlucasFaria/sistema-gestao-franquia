using Franquias.Api.Common.Autenticacao;
using Franquias.Api.DTOs.Autenticacao;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Autenticação de usuários e cadastro de contas de acesso.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IAuthService autenticacao) : ControllerBase
{
    /// <summary>
    /// Autentica um usuário e devolve o token de acesso.
    /// </summary>
    /// <param name="requisicao">E-mail e senha cadastrados.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Autenticado. O token deve ser enviado como <c>Authorization: Bearer</c>.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="401">Credenciais inválidas ou usuário inativo.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Entrar(
        [FromBody] LoginRequest requisicao,
        CancellationToken cancellationToken)
    {
        var resposta = await autenticacao.AutenticarAsync(requisicao, cancellationToken);

        return Ok(resposta);
    }

    /// <summary>
    /// Cadastra um novo usuário da rede e devolve o token do primeiro acesso.
    /// </summary>
    /// <param name="requisicao">Dados do usuário e o perfil de acesso a conceder.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Usuário cadastrado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="401">Requisição sem token de acesso.</response>
    /// <response code="403">Perfil sem permissão para cadastrar usuários.</response>
    /// <response code="404">Perfil de acesso inexistente.</response>
    /// <response code="409">Já existe usuário com o e-mail informado.</response>
    [HttpPost("registrar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoginResponse>> Registrar(
        [FromBody] RegistroRequest requisicao,
        CancellationToken cancellationToken)
    {
        var resposta = await autenticacao.RegistrarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(Registrar), new { id = resposta.UsuarioId }, resposta);
    }
}
