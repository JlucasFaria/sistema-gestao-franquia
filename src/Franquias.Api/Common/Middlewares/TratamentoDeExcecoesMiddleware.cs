using System.Diagnostics;
using Franquias.Api.Common.Excecoes;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Common.Middlewares;

/// <summary>
/// Converte exceções não tratadas em respostas <c>application/problem+json</c> no formato
/// ProblemDetails (RFC 7807), com código HTTP coerente com a natureza da falha.
/// </summary>
public sealed class TratamentoDeExcecoesMiddleware(
    RequestDelegate proximo,
    ILogger<TratamentoDeExcecoesMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await proximo(contexto);
        }
        catch (Exception excecao)
        {
            await TratarAsync(contexto, excecao);
        }
    }

    private async Task TratarAsync(HttpContext contexto, Exception excecao)
    {
        // Se a resposta já começou a ser enviada, não há como trocar status nem corpo:
        // sobrescrever aqui produziria um payload corrompido no cliente.
        if (contexto.Response.HasStarted)
        {
            logger.LogError(excecao, "Falha após o início da resposta; a requisição será encerrada.");
            throw excecao;
        }

        var (status, titulo) = Mapear(excecao);

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                excecao,
                "Falha não tratada ao processar {Metodo} {Caminho}.",
                contexto.Request.Method,
                contexto.Request.Path);
        }
        else
        {
            logger.LogWarning(
                "Requisição recusada em {Metodo} {Caminho}: {Mensagem}",
                contexto.Request.Method,
                contexto.Request.Path,
                excecao.Message);
        }

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Instance = contexto.Request.Path,

            // Mensagem interna nunca vaza em erro inesperado: pode conter caminho de
            // arquivo, SQL ou detalhe de implementação.
            Detail = status == StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado ao processar a requisição."
                : excecao.Message
        };

        // Correlaciona a resposta com a entrada de log correspondente.
        problema.Extensions["traceId"] = Activity.Current?.Id ?? contexto.TraceIdentifier;

        contexto.Response.Clear();
        contexto.Response.StatusCode = status;

        // O content type precisa ir no próprio WriteAsJsonAsync: definido apenas na
        // resposta, ele seria sobrescrito por "application/json" na serialização.
        await contexto.Response.WriteAsJsonAsync(
            problema,
            options: null,
            contentType: "application/problem+json");
    }

    /// <summary>
    /// Traduz a exceção no par código HTTP e título. Exceções fora do domínio caem no 500
    /// de propósito: são defeito do sistema, não pedido inválido do cliente.
    /// </summary>
    private static (int Status, string Titulo) Mapear(Exception excecao) => excecao switch
    {
        CredenciaisInvalidasException => (StatusCodes.Status401Unauthorized, "Não autenticado"),
        NaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
        ConflitoException => (StatusCodes.Status409Conflict, "Conflito com um registro existente"),
        RegraDeNegocioException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
        DominioException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
    };
}
