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

        builder.Property(r => r.FaturamentoBase)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.PercentualAplicado)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(r => r.ValorDevido)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.ValorPago)
            .HasPrecision(18, 2);

        builder.HasOne(r => r.UnidadeFranqueada)
            .WithMany()
            .HasForeignKey(r => r.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Impede emitir duas cobranças para a mesma unidade no mesmo período de apuração.
        builder.HasIndex(r => new { r.UnidadeFranqueadaId, r.PeriodoInicio, r.PeriodoFim })
            .IsUnique();

        // Sustenta a consulta de valores devidos e pagos por unidade.
        builder.HasIndex(r => new { r.UnidadeFranqueadaId, r.Situacao });
    }
}
