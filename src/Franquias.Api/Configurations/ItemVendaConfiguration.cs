using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="ItemVenda"/>.</summary>
public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("ItensVenda");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantidade)
            .IsRequired();

        builder.Property(i => i.PrecoUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        // Item do catálogo com venda registrada não pode ser apagado.
        builder.HasOne(i => i.ProdutoServico)
            .WithMany()
            .HasForeignKey(i => i.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sustenta o relatório de produtos mais vendidos.
        builder.HasIndex(i => i.ProdutoServicoId);
    }
}
