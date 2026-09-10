using System.Text;
using Franquias.Api.Common.Autenticacao;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api.Common.Injecao;

/// <summary>
/// Configuração da autenticação por Bearer Token.
/// </summary>
public static class ExtensoesDeAutenticacao
{
    /// <summary>
    /// Vincula a seção <c>Jwt</c> da configuração e habilita a validação dos tokens
    /// recebidos, usando os mesmos emissor, audiência e chave da emissão.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se a chave de assinatura não estiver configurada.</exception>
    public static IServiceCollection AdicionarAutenticacao(
        this IServiceCollection servicos,
        IConfiguration configuracao)
    {
        var secao = configuracao.GetSection(OpcoesDeJwt.Secao);
        servicos.Configure<OpcoesDeJwt>(secao);

        var opcoes = secao.Get<OpcoesDeJwt>() ?? new OpcoesDeJwt();

        // Falha na inicialização, e não na primeira requisição de login: sem chave, todo
        // token emitido seria inseguro ou a emissão quebraria em runtime.
        if (string.IsNullOrWhiteSpace(opcoes.ChaveSecreta))
        {
            throw new InvalidOperationException(
                "A chave de assinatura do JWT não está configurada. Defina 'Jwt:ChaveSecreta' "
                + "no arquivo de configuração ou na variável de ambiente 'Jwt__ChaveSecreta'.");
        }

        servicos
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(parametros =>
            {
                parametros.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = opcoes.Emissor,

                    ValidateAudience = true,
                    ValidAudience = opcoes.Audiencia,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(opcoes.ChaveSecreta)),

                    ValidateLifetime = true,

                    // Sem tolerância: o padrão de 5 minutos faria um token expirado
                    // continuar sendo aceito depois do prazo declarado ao cliente.
                    ClockSkew = TimeSpan.Zero
                };
            });

        return servicos;
    }
}
