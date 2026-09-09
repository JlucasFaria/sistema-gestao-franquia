using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Franqueado"/>.</summary>
public class FranqueadoConfiguration : IEntityTypeConfiguration<Franqueado>
{
    public void Configure(EntityTypeBuilder<Franqueado> builder)
    {
        builder.ToTable("Franqueados");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(f => f.Cpf)
            .HasMaxLength(11)
            .IsFixedLength()
            .IsRequired();

        builder.Property(f => f.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(f => f.Telefone)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(f => f.DataAdesao)
            .IsRequired();
    }
}
