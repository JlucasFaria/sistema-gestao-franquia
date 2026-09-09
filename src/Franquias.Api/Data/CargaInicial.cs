using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

/// <summary>
/// Carga inicial do banco, aplicada pela migration. Os dados aqui precisam ser constantes:
/// o EF Core compara este conteúdo com o snapshot do modelo para decidir o que inserir, de
/// modo que qualquer valor calculado em tempo de execução geraria uma migration nova a cada
/// build.
/// </summary>
public static class CargaInicial
{
    /// <summary>
    /// Data de referência fixa dos registros semeados. Não pode ser <c>DateTime.UtcNow</c>.
    /// </summary>
    private static readonly DateTime Referencia = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Hash BCrypt da senha do administrador, gerado uma única vez. O BCrypt usa salt
    /// aleatório, então calcular o hash aqui produziria um valor diferente a cada execução
    /// e a migration nunca estabilizaria.
    /// </summary>
    private const string SenhaHashAdministrador =
        "$2a$11$b26efR43sVZtVMymT7DMvuqI4FvaLSEtSV0QfROWfIENmdoXSRlEW";

    /// <summary>
    /// Declara os dados mínimos para a aplicação subir utilizável: os perfis de acesso, um
    /// administrador para o primeiro login, a franqueadora da rede e as categorias base.
    /// </summary>
    public static void Aplicar(ModelBuilder modelBuilder)
    {
        SemearPerfis(modelBuilder);
        SemearAdministrador(modelBuilder);
        SemearFranqueadora(modelBuilder);
        SemearCategorias(modelBuilder);
    }

    private static void SemearPerfis(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Perfil>().HasData(
            new
            {
                Id = 1,
                Codigo = PerfilAcesso.Administrador,
                Nome = "Administrador",
                Descricao = "Acesso total à rede, incluindo cadastros da franqueadora e relatórios consolidados.",
                DataCriacao = Referencia,
                Ativo = true
            },
            new
            {
                Id = 2,
                Codigo = PerfilAcesso.GestorUnidade,
                Nome = "Gestor de Unidade",
                Descricao = "Gerencia estoque, vendas e chamados da própria unidade franqueada.",
                DataCriacao = Referencia,
                Ativo = true
            },
            new
            {
                Id = 3,
                Codigo = PerfilAcesso.Operador,
                Nome = "Operador",
                Descricao = "Acesso operacional ao registro de vendas e às movimentações de estoque.",
                DataCriacao = Referencia,
                Ativo = true
            });

    private static void SemearAdministrador(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Usuario>().HasData(
            new
            {
                Id = 1,
                Nome = "Administrador do Sistema",
                Email = "admin@franquias.com.br",
                SenhaHash = SenhaHashAdministrador,
                PerfilId = 1,
                DataCriacao = Referencia,
                Ativo = true
            });

    private static void SemearFranqueadora(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Franqueadora>().HasData(
            new
            {
                Id = 1,
                RazaoSocial = "Rede Sabor Brasil Franquias LTDA",
                NomeFantasia = "Sabor Brasil",
                Cnpj = "11222333000181",
                Email = "contato@saborbrasil.com.br",
                Telefone = "1133334444",
                DataCriacao = Referencia,
                Ativo = true
            });

        modelBuilder.Entity<Franqueadora>()
            .OwnsOne(f => f.Endereco)
            .HasData(
                new
                {
                    FranqueadoraId = 1,
                    Logradouro = "Avenida Paulista",
                    Numero = "1000",
                    Complemento = "10 andar",
                    Bairro = "Bela Vista",
                    Cidade = "Sao Paulo",
                    Uf = "SP",
                    Cep = "01310100"
                });
    }

    private static void SemearCategorias(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Categoria>().HasData(
            new
            {
                Id = 1,
                Nome = "Alimentos",
                Descricao = "Itens alimentícios preparados ou industrializados.",
                DataCriacao = Referencia,
                Ativo = true
            },
            new
            {
                Id = 2,
                Nome = "Bebidas",
                Descricao = "Bebidas quentes, geladas e alcoólicas.",
                DataCriacao = Referencia,
                Ativo = true
            },
            new
            {
                Id = 3,
                Nome = "Insumos",
                Descricao = "Matérias-primas e embalagens usadas na operação da unidade.",
                DataCriacao = Referencia,
                Ativo = true
            },
            new
            {
                Id = 4,
                Nome = "Serviços",
                Descricao = "Serviços prestados pelas unidades aos clientes da rede.",
                DataCriacao = Referencia,
                Ativo = true
            });
}
