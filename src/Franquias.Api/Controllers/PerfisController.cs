using Franquias.Api.Common.Autenticacao;
using Franquias.Api.DTOs.Perfis;
using Franquias.Api.DTOs.Usuarios;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Consulta dos perfis de acesso e vínculo de perfis a usuários.
/// </summary>
[ApiController]
[Route("api/perfis")]
[Produces("application/json")]
public class PerfisController(IPerfilService perfis, IUsuarioService usuarios) : ControllerBase
{
    /// <summary>
    /// Lista os perfis de acesso disponíveis.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Perfis cadastrados.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PerfilResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<PerfilResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var resultado = await perfis.ListarAsync(cancellationToken);

        return Ok(resultado);
    }

    /// <summary>
    /// Busca um perfil de acesso pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do perfil.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Perfil encontrado.</response>
    /// <response code="404">Perfil inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Operador)]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PerfilResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var perfil = await perfis.ObterPorIdAsync(id, cancellationToken);

        return Ok(perfil);
    }

    /// <summary>
    /// Vincula um perfil de acesso a um usuário, substituindo o anterior.
    /// </summary>
    /// <param name="usuarioId">Identificador do usuário.</param>
    /// <param name="requisicao">Perfil a ser concedido.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Perfil vinculado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Usuário ou perfil inexistente.</response>
    /// <response code="422">Tentativa de rebaixar o último administrador ativo da rede.</response>
    [HttpPut("usuarios/{usuarioId:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UsuarioResponse>> VincularAUsuario(
        int usuarioId,
        [FromBody] VincularPerfilRequest requisicao,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarios.VincularPerfilAsync(
            usuarioId,
            requisicao.PerfilId,
            cancellationToken);

        return Ok(usuario);
    }
}
