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

        builder.HasOne(i => i.ProdutoServico)
            .WithMany()
            .HasForeignKey(i => i.ProdutoServicoId);

        // Sustenta o relatório de produtos mais vendidos.
        builder.HasIndex(i => i.ProdutoServicoId);
    }
}
