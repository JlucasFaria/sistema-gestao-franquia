using Franquias.Api.Common.Injecao;
using Franquias.Api.Common.Middlewares;
using Franquias.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AdicionarPersistencia(builder.Configuration)
    .AdicionarRepositorios()
    .AdicionarServicosDeAplicacao()
    .AdicionarDocumentacao();

builder.Services.AddControllers();

var app = builder.Build();

app.UsarTratamentoDeExcecoes();

await app.PrepararBancoAsync();

if (app.Environment.IsDevelopment())
{
    app.UsarDocumentacao();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
