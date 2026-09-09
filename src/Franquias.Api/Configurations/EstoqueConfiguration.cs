using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Estoque"/>.</summary>
public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.ToTable("Estoques");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Quantidade)
            .IsRequired();

        builder.Property(e => e.QuantidadeMinima)
            .IsRequired();

        builder.HasOne(e => e.UnidadeFranqueada)
            .WithMany()
            .HasForeignKey(e => e.UnidadeFranqueadaId);

        builder.HasOne(e => e.ProdutoServico)
            .WithMany()
            .HasForeignKey(e => e.ProdutoServicoId);

        builder.HasMany(e => e.Movimentacoes)
            .WithOne(m => m.Estoque)
            .HasForeignKey(m => m.EstoqueId);

        builder.Navigation(e => e.Movimentacoes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
