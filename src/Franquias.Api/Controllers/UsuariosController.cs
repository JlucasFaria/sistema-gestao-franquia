using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Usuarios;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e consulta dos usuários da rede.
/// </summary>
[ApiController]
[Route("api/usuarios")]
[Produces("application/json")]
public class UsuariosController(IUsuarioService usuarios) : ControllerBase
{
    /// <summary>
    /// Lista os usuários de forma paginada.
    /// </summary>
    /// <param name="parametros">Página, tamanho, ordenação e termo de busca por nome ou e-mail.</param>
    /// <param name="apenasAtivos">Filtra por situação. Omitido, traz ativos e inativos.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Página de usuários.</response>
    [HttpGet]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(PagedResult<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UsuarioResponse>>> Listar(
        [FromQuery] QueryParams parametros,
        [FromQuery] bool? apenasAtivos,
        CancellationToken cancellationToken)
    {
        var pagina = await usuarios.ListarAsync(parametros, apenasAtivos, cancellationToken);

        return Ok(pagina);
    }

    /// <summary>
    /// Busca um usuário pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Usuário encontrado.</response>
    /// <response code="404">Usuário inexistente.</response>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.GestorDeUnidade)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> ObterPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarios.ObterPorIdAsync(id, cancellationToken);

        return Ok(usuario);
    }

    /// <summary>
    /// Cadastra um usuário.
    /// </summary>
    /// <param name="requisicao">Dados do usuário e perfil de acesso.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Usuário cadastrado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Perfil de acesso inexistente.</response>
    /// <response code="409">Já existe usuário com o e-mail informado.</response>
    [HttpPost]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Criar(
        [FromBody] CriarUsuarioRequest requisicao,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarios.CriarAsync(requisicao, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    /// <summary>
    /// Altera nome e e-mail de um usuário.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="requisicao">Novos dados cadastrais.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Usuário atualizado.</response>
    /// <response code="400">Dados da requisição inválidos.</response>
    /// <response code="404">Usuário inexistente.</response>
    /// <response code="409">O e-mail informado já pertence a outro usuário.</response>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Atualizar(
        int id,
        [FromBody] AtualizarUsuarioRequest requisicao,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarios.AtualizarAsync(id, requisicao, cancellationToken);

        return Ok(usuario);
    }

    /// <summary>
    /// Reativa um usuário, devolvendo-lhe o acesso ao sistema.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Usuário ativo. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Usuário inexistente.</response>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> Ativar(
        int id,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarios.AtivarAsync(id, cancellationToken);

        return Ok(usuario);
    }

    /// <summary>
    /// Inativa um usuário, bloqueando a autenticação sem apagar o cadastro nem o histórico.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Usuário inativo. Repetir a chamada não produz efeito adicional.</response>
    /// <response code="404">Usuário inexistente.</response>
    /// <response code="422">Tentativa de inativar o último administrador ativo da rede.</response>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Policy = PoliticasDeAcesso.Administrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UsuarioResponse>> Inativar(
        int id,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarios.InativarAsync(id, cancellationToken);

        return Ok(usuario);
    }
}
