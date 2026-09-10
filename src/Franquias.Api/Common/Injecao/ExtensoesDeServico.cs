using Franquias.Api.Data;
using Franquias.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Common.Injecao;

/// <summary>
/// Registro das dependências da aplicação, agrupado por responsabilidade para manter o
/// <c>Program.cs</c> legível conforme o número de serviços cresce.
/// </summary>
public static class ExtensoesDeServico
{
    /// <summary>Sufixo dos repositórios, usado no registro por convenção.</summary>
    private const string SufixoRepositorio = "Repositorio";

    /// <summary>Sufixo dos serviços de aplicação, usado no registro por convenção.</summary>
    private const string SufixoServico = "Service";

    /// <summary>
    /// Registra o contexto de persistência sobre o provider SQLite.
    /// </summary>
    public static IServiceCollection AdicionarPersistencia(
        this IServiceCollection servicos,
        IConfiguration configuracao)
    {
        servicos.AddDbContext<AppDbContext>(opcoes =>
            opcoes.UseSqlite(configuracao.GetConnectionString("ConexaoPadrao")));

        return servicos;
    }

    /// <summary>
    /// Registra o repositório genérico como tipo aberto e, por convenção, os repositórios
    /// específicos de cada entidade.
    /// </summary>
    public static IServiceCollection AdicionarRepositorios(this IServiceCollection servicos)
    {
        // Tipo aberto: uma única linha atende IRepositorio<Usuario>, IRepositorio<Venda>
        // e qualquer outra entidade, sem registro por tipo.
        servicos.AddScoped(typeof(IRepositorio<>), typeof(RepositorioGenerico<>));

        RegistrarPorConvencao(servicos, SufixoRepositorio);

        return servicos;
    }

    /// <summary>
    /// Registra, por convenção, os serviços de aplicação de cada grupo funcional.
    /// </summary>
    public static IServiceCollection AdicionarServicosDeAplicacao(this IServiceCollection servicos)
    {
        RegistrarPorConvencao(servicos, SufixoServico);

        return servicos;
    }

    /// <summary>
    /// Associa cada classe concreta terminada no sufixo à sua interface de mesmo nome
    /// prefixada por "I" — <c>UsuarioService</c> para <c>IUsuarioService</c> — com tempo de
    /// vida <c>Scoped</c>, o mesmo do <see cref="AppDbContext"/>, para que todos os
    /// colaboradores de uma requisição compartilhem a mesma unidade de trabalho.
    /// </summary>
    private static void RegistrarPorConvencao(IServiceCollection servicos, string sufixo)
    {
        var assembly = typeof(ExtensoesDeServico).Assembly;

        var implementacoes = assembly
            .GetTypes()
            .Where(tipo => tipo is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
            .Where(tipo => tipo.Name.EndsWith(sufixo, StringComparison.Ordinal));

        foreach (var implementacao in implementacoes)
        {
            var contrato = Array.Find(
                implementacao.GetInterfaces(),
                interfaceCandidata => interfaceCandidata.Name == $"I{implementacao.Name}");

            if (contrato is not null)
            {
                servicos.AddScoped(contrato, implementacao);
            }
        }
    }
}
