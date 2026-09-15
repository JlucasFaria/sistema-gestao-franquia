using System.Globalization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Franquias.Api.Common.Excecoes;

namespace Franquias.Api.Common.Autenticacao;

/// <summary>
/// Leitura dos dados do usuário autenticado a partir do token apresentado na requisição.
/// </summary>
public static class ExtensoesDeClaims
{
    /// <summary>
    /// Devolve o identificador do usuário dono do token. O identificador é emitido na claim
    /// padrão <c>sub</c>, que o ASP.NET Core traduz para <see cref="ClaimTypes.NameIdentifier"/>.
    /// </summary>
    /// <exception cref="CredenciaisInvalidasException">
    /// Se o token não trouxer um identificador numérico utilizável.
    /// </exception>
    public static int ObterIdDoUsuario(this ClaimsPrincipal usuario)
    {
        var identificador = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? usuario.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!int.TryParse(identificador, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
        {
            throw new CredenciaisInvalidasException("O token não identifica o usuário autenticado.");
        }

        return id;
    }
}
