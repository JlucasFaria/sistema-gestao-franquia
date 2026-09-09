using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Venda"/>.</summary>
public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.DataVenda)
            .IsRequired();

        builder.Property(v => v.Status)
            .IsRequired();

        builder.Property(v => v.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        // Unidade com vendas registradas não pode ser apagada: o histórico financeiro depende dela.
        builder.HasOne(v => v.UnidadeFranqueada)
            .WithMany()
            .HasForeignKey(v => v.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.Itens)
            .WithOne(i => i.Venda)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Sustenta a consulta por unidade e intervalo de datas e a apuração de faturamento.
        builder.HasIndex(v => new { v.UnidadeFranqueadaId, v.DataVenda });
    }
}
