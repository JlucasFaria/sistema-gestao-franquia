using Franquias.Api.Common.Autenticacao;
using Franquias.Api.Entities.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Franquias.Api.Common.Injecao;

/// <summary>
/// Configuração das políticas de autorização por perfil de acesso.
/// </summary>
public static class ExtensoesDeAutorizacao
{
    private const string Administrador = nameof(PerfilAcesso.Administrador);
    private const string GestorUnidade = nameof(PerfilAcesso.GestorUnidade);
    private const string Operador = nameof(PerfilAcesso.Operador);

    /// <summary>
    /// Declara as políticas usadas pelos controllers e exige autenticação por padrão em
    /// qualquer endpoint que não declare o contrário.
    /// </summary>
    public static IServiceCollection AdicionarAutorizacao(this IServiceCollection servicos)
    {
        servicos
            .AddAuthorizationBuilder()

            // Cada política aceita também os perfis acima dela na hierarquia. Sem isso, um
            // administrador receberia 403 em endpoints de operação, o que seria absurdo.
            .AddPolicy(PoliticasDeAcesso.Administrador, politica =>
                politica.RequireRole(Administrador))

            .AddPolicy(PoliticasDeAcesso.GestorDeUnidade, politica =>
                politica.RequireRole(Administrador, GestorUnidade))

            .AddPolicy(PoliticasDeAcesso.Operador, politica =>
                politica.RequireRole(Administrador, GestorUnidade, Operador))

            // Endpoint sem atributo de autorização passa a exigir autenticação. Um controller
            // novo nasce protegido; esquecer o [Authorize] deixa de ser uma brecha silenciosa.
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return servicos;
    }
}
