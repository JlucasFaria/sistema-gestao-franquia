using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Usuarios;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio de usuários da rede.
/// </summary>
public interface IUsuarioService
{
    /// <summary>
    /// Lista os usuários de forma paginada, com busca por nome ou e-mail.
    /// </summary>
    /// <param name="parametros">Paginação, ordenação e termo de busca.</param>
    /// <param name="apenasAtivos">Filtra por situação. Nulo traz ativos e inativos.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<PagedResult<UsuarioResponse>> ListarAsync(
        QueryParams parametros,
        bool? apenasAtivos,
        CancellationToken cancellationToken = default);

    /// <summary>Busca um usuário pelo identificador.</summary>
    Task<UsuarioResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Cadastra um usuário, recusando e-mail já em uso.</summary>
    Task<UsuarioResponse> CriarAsync(
        CriarUsuarioRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera nome e e-mail de um usuário, recusando e-mail já em uso por outro.</summary>
    Task<UsuarioResponse> AtualizarAsync(
        int id,
        AtualizarUsuarioRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reativa um usuário, devolvendo-lhe o acesso. Operação idempotente.
    /// </summary>
    Task<UsuarioResponse> AtivarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inativa um usuário, sem removê-lo da base, bloqueando a autenticação. Operação
    /// idempotente.
    /// </summary>
    Task<UsuarioResponse> InativarAsync(int id, CancellationToken cancellationToken = default);
}
