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
