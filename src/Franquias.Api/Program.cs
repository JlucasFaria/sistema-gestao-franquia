using Franquias.Api.Common.Injecao;
using Franquias.Api.Common.Middlewares;
using Franquias.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AdicionarPersistencia(builder.Configuration)
    .AdicionarRepositorios()
    .AdicionarServicosDeAplicacao();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UsarTratamentoDeExcecoes();

await app.PrepararBancoAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
