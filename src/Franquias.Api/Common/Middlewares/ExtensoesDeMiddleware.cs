namespace Franquias.Api.Common.Middlewares;

/// <summary>
/// Registro dos middlewares próprios da aplicação no pipeline.
/// </summary>
public static class ExtensoesDeMiddleware
{
    /// <summary>
    /// Habilita o tratamento global de exceções. Deve ser o primeiro middleware do
    /// pipeline, para envolver todos os que vierem depois.
    /// </summary>
    public static IApplicationBuilder UsarTratamentoDeExcecoes(this IApplicationBuilder app) =>
        app.UseMiddleware<TratamentoDeExcecoesMiddleware>();
}
