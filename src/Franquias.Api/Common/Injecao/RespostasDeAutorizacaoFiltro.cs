using Franquias.Api.Common.Autenticacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Franquias.Api.Common.Injecao;

/// <summary>
/// Completa a documentação de cada endpoint protegido: exige o token só onde ele é
/// necessário e descreve as respostas 401 e 403, que não são declaradas em cada ação.
/// </summary>
public sealed class RespostasDeAutorizacaoFiltro : IOperationFilter
{
    private readonly OpenApiSecurityScheme _esquemaBearer;

    public RespostasDeAutorizacaoFiltro(string esquemaBearer)
    {
        _esquemaBearer = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = esquemaBearer
            }
        };
    }

    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadados = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        if (metadados.OfType<IAllowAnonymous>().Any())
        {
            return;
        }

        operation.Security =
        [
            new OpenApiSecurityRequirement { { _esquemaBearer, Array.Empty<string>() } }
        ];

        operation.Responses.TryAdd(
            StatusCodes.Status401Unauthorized.ToString(),
            new OpenApiResponse { Description = "Token ausente, inválido ou expirado." });

        // O 403 só é possível quando a política exige mais que o perfil de operador, que é
        // atendido por qualquer usuário autenticado.
        var exigePerfilSuperior = metadados
            .OfType<IAuthorizeData>()
            .Any(autorizacao => autorizacao.Policy is PoliticasDeAcesso.Administrador
                or PoliticasDeAcesso.GestorDeUnidade);

        if (exigePerfilSuperior)
        {
            operation.Responses.TryAdd(
                StatusCodes.Status403Forbidden.ToString(),
                new OpenApiResponse { Description = "O perfil do usuário não tem permissão para esta operação." });
        }
    }
}
