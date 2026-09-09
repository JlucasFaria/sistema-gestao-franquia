using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Franquias.Api.Data;

/// <summary>
/// Contexto de persistência da aplicação. Expõe os conjuntos de entidades e aplica os
/// mapeamentos declarados nas classes de configuração do assembly.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Perfil> Perfis => Set<Perfil>();

    public DbSet<Franqueadora> Franqueadoras => Set<Franqueadora>();

    public DbSet<Franqueado> Franqueados => Set<Franqueado>();

    public DbSet<Responsavel> Responsaveis => Set<Responsavel>();

    public DbSet<UnidadeFranqueada> Unidades => Set<UnidadeFranqueada>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<ProdutoServico> Produtos => Set<ProdutoServico>();

    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    public DbSet<FornecedorProduto> FornecedoresProdutos => Set<FornecedorProduto>();

    public DbSet<Estoque> Estoques => Set<Estoque>();

    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    public DbSet<Venda> Vendas => Set<Venda>();

    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();

    public DbSet<Royalty> Royalties => Set<Royalty>();

    public DbSet<ChamadoSuporte> Chamados => Set<ChamadoSuporte>();

    public DbSet<InteracaoChamado> InteracoesChamado => Set<InteracaoChamado>();

    /// <inheritdoc />
    public override int SaveChanges()
    {
        AplicarAuditoria();
        return base.SaveChanges();
    }

    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AplicarAuditoria();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Depois dos mapeamentos: a carga inicial depende das configurações já aplicadas.
        CargaInicial.Aplicar(modelBuilder);
    }

    /// <summary>
    /// Carimba as datas de auditoria a partir do rastreador de mudanças, de modo que
    /// nenhuma escrita dependa de o serviço lembrar de fazê-lo.
    /// </summary>
    private void AplicarAuditoria()
    {
        var agora = DateTime.UtcNow;

        foreach (EntityEntry<EntidadeBase> entrada in ChangeTracker.Entries<EntidadeBase>())
        {
            switch (entrada.State)
            {
                case EntityState.Added:
                    entrada.Property(nameof(EntidadeBase.DataCriacao)).CurrentValue = agora;
                    break;

                case EntityState.Modified:
                    entrada.Property(nameof(EntidadeBase.DataCriacao)).IsModified = false;
                    entrada.Property(nameof(EntidadeBase.DataAtualizacao)).CurrentValue = agora;
                    break;
            }
        }
    }
}
