using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Royalty"/>.</summary>
public class RoyaltyConfiguration : IEntityTypeConfiguration<Royalty>
{
    public void Configure(EntityTypeBuilder<Royalty> builder)
    {
        builder.ToTable("Royalties");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.PeriodoInicio)
            .IsRequired();

        builder.Property(r => r.PeriodoFim)
            .IsRequired();

        builder.Property(r => r.DataVencimento)
            .IsRequired();

        builder.Property(r => r.Situacao)
            .IsRequired();

        builder.HasOne(r => r.UnidadeFranqueada)
            .WithMany()
            .HasForeignKey(r => r.UnidadeFranqueadaId);

        // Sustenta a consulta de valores devidos e pagos por unidade.
        builder.HasIndex(r => new { r.UnidadeFranqueadaId, r.Situacao });
    }
}
