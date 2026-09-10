using System.Text.Json.Serialization;
using Franquias.Api.Common.Injecao;
using Franquias.Api.Common.Middlewares;
using Franquias.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AdicionarPersistencia(builder.Configuration)
    .AdicionarRepositorios()
    .AdicionarServicosDeAplicacao()
    .AdicionarAutenticacao(builder.Configuration)
    .AdicionarAutorizacao()
    .AdicionarDocumentacao();

builder.Services
    .AddControllers()
    .AddJsonOptions(opcoes =>
        // Enums trafegam pelo nome ("Ativa"), não pelo número: a resposta fica legível e
        // o cliente não quebra se a ordem dos membros do enum mudar.
        opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.UsarTratamentoDeExcecoes();

await app.PrepararBancoAsync();

if (app.Environment.IsDevelopment())
{
    app.UsarDocumentacao();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
