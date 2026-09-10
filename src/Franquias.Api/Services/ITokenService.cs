using Franquias.Api.Entities;

namespace Franquias.Api.Services;

/// <summary>
/// Emissão dos tokens JWT usados para autenticar as requisições.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Emite um token para o usuário, incluindo o perfil de acesso nas claims.
    /// </summary>
    /// <param name="usuario">
    /// Usuário autenticado, com a navegação <see cref="Usuario.Perfil"/> carregada.
    /// </param>
    TokenGerado Gerar(Usuario usuario);
}

/// <summary>
/// Token emitido e o instante em que deixa de valer.
/// </summary>
/// <param name="Token">Token JWT compacto.</param>
/// <param name="ExpiraEm">Momento da expiração, em UTC.</param>
public sealed record TokenGerado(string Token, DateTime ExpiraEm);
