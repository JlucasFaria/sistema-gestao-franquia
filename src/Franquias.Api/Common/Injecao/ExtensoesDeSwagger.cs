using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Franquias.Api.Common.Injecao;

/// <summary>
/// Configuração da documentação OpenAPI gerada pelo Swashbuckle.
/// </summary>
public static class ExtensoesDeSwagger
{
    private const string NomeDoDocumento = "v1";
    private const string TituloDaApi = "API de Gestão de Franquias";
    private const string EsquemaBearer = "Bearer";

    /// <summary>
    /// Registra o gerador da especificação OpenAPI, incluindo os comentários XML do
    /// projeto para que descrições e parâmetros apareçam na documentação.
    /// </summary>
    public static IServiceCollection AdicionarDocumentacao(this IServiceCollection servicos)
    {
        servicos.AddEndpointsApiExplorer();

        servicos.AddSwaggerGen(opcoes =>
        {
            opcoes.SwaggerDoc(NomeDoDocumento, new OpenApiInfo
            {
                Title = TituloDaApi,
                Version = NomeDoDocumento,
                Description =
                    "Gestão de uma rede de franquias: unidades franqueadas, catálogo de produtos e " +
                    "serviços, fornecedores, estoque por unidade, vendas, royalties, chamados de " +
                    "suporte e relatórios gerenciais."
            });

            IncluirComentariosXml(opcoes);
            ConfigurarAutenticacaoBearer(opcoes);
        });

        return servicos;
    }

    /// <summary>
    /// Publica a especificação e a interface interativa do Swagger.
    /// </summary>
    public static WebApplication UsarDocumentacao(this WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(opcoes =>
        {
            opcoes.SwaggerEndpoint($"/swagger/{NomeDoDocumento}/swagger.json", $"{TituloDaApi} {NomeDoDocumento}");
            opcoes.DocumentTitle = TituloDaApi;
        });

        return app;
    }

    /// <summary>
    /// Declara o esquema Bearer, para que a interface do Swagger ofereça o botão de
    /// autorização e passe a enviar o token nas requisições de teste.
    /// </summary>
    private static void ConfigurarAutenticacaoBearer(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions opcoes)
    {
        opcoes.AddSecurityDefinition(EsquemaBearer, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Cole apenas o token devolvido por POST /api/auth/login. "
                + "O prefixo 'Bearer ' é acrescentado automaticamente."
        });

        opcoes.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = EsquemaBearer
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    private static void IncluirComentariosXml(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions opcoes)
    {
        var nomeDoArquivo = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var caminho = Path.Combine(AppContext.BaseDirectory, nomeDoArquivo);

        // Sem o arquivo, o Swagger ainda funciona — apenas sem as descrições. Verificar
        // evita derrubar a aplicação por causa de um artefato de build ausente.
        if (File.Exists(caminho))
        {
            opcoes.IncludeXmlComments(caminho, includeControllerXmlComments: true);
        }
    }
}
