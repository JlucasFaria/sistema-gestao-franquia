using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="ProdutoServico"/>.</summary>
public class ProdutoServicoConfiguration : IEntityTypeConfiguration<ProdutoServico>
{
    public void Configure(EntityTypeBuilder<ProdutoServico> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.EhServico)
            .IsRequired();

        builder.Property(p => p.PrecoBase)
            .HasPrecision(18, 2)
            .IsRequired();

        // Categoria com itens vinculados não pode ser apagada.
        builder.HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Nome);
    }
}
