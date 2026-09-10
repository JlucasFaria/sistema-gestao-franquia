namespace Franquias.Api.DTOs.Autenticacao;

/// <summary>
/// Resposta de uma autenticação bem-sucedida.
/// </summary>
/// <param name="Token">Token JWT a ser enviado no cabeçalho <c>Authorization: Bearer</c>.</param>
/// <param name="ExpiraEm">Momento da expiração do token, em UTC.</param>
/// <param name="UsuarioId">Identificador do usuário autenticado.</param>
/// <param name="Nome">Nome do usuário autenticado.</param>
/// <param name="Email">E-mail do usuário autenticado.</param>
/// <param name="Perfil">Perfil de acesso concedido.</param>
public sealed record LoginResponse(
    string Token,
    DateTime ExpiraEm,
    int UsuarioId,
    string Nome,
    string Email,
    string Perfil);
