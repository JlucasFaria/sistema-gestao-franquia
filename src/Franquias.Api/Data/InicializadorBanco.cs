using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

/// <summary>
/// Preparação do banco de dados durante a inicialização da aplicação.
/// </summary>
public static class InicializadorBanco
{
    /// <summary>
    /// Aplica as migrations pendentes, criando o banco e a carga inicial caso ainda não
    /// existam. Deixa a aplicação utilizável logo após o clone do repositório, sem exigir
    /// que se rode o <c>dotnet ef database update</c> à mão.
    /// </summary>
    public static async Task PrepararBancoAsync(this WebApplication app)
    {
        using var escopo = app.Services.CreateScope();

        var contexto = escopo.ServiceProvider.GetRequiredService<AppDbContext>();
        var fabricaDeLog = escopo.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = fabricaDeLog.CreateLogger(nameof(InicializadorBanco));

        var pendentes = (await contexto.Database.GetPendingMigrationsAsync()).ToList();

        if (pendentes.Count == 0)
        {
            logger.LogInformation("Banco de dados já está atualizado; nenhuma migration pendente.");
            return;
        }

        logger.LogInformation(
            "Aplicando {Quantidade} migration(s) pendente(s): {Migrations}.",
            pendentes.Count,
            string.Join(", ", pendentes));

        await contexto.Database.MigrateAsync();

        logger.LogInformation("Banco de dados atualizado com sucesso.");
    }
}
