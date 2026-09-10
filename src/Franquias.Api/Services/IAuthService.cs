using Franquias.Api.DTOs.Autenticacao;

namespace Franquias.Api.Services;

/// <summary>
/// Autenticação de usuários e cadastro de novas contas de acesso.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Valida as credenciais e emite o token de acesso.
    /// </summary>
    Task<LoginResponse> AutenticarAsync(
        LoginRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cadastra um novo usuário e devolve o token do primeiro acesso.
    /// </summary>
    Task<LoginResponse> RegistrarAsync(
        RegistroRequest requisicao,
        CancellationToken cancellationToken = default);
}
