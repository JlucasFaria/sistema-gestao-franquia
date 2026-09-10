using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="ITokenService"/> com assinatura HMAC-SHA256.
/// </summary>
public sealed class TokenService(IOptions<OpcoesDeJwt> opcoes) : ITokenService
{
    private readonly OpcoesDeJwt _opcoes = opcoes.Value;

    /// <inheritdoc />
    public TokenGerado Gerar(Usuario usuario)
    {
        if (usuario.Perfil is null)
        {
            throw new InvalidOperationException(
                "O perfil do usuário precisa estar carregado para emitir o token com a claim de acesso.");
        }

        var expiraEm = DateTime.UtcNow.AddMinutes(_opcoes.ExpiracaoEmMinutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString(CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Name, usuario.Nome),

            // Identificador único do token, útil para rastrear e, se preciso, revogar.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // É esta claim que as políticas de autorização por perfil consultam.
            new(ClaimTypes.Role, usuario.Perfil.Codigo.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.ChaveSecreta));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opcoes.Emissor,
            audience: _opcoes.Audiencia,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenGerado(new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
