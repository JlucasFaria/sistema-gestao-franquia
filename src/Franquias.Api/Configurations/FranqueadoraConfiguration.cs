using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Franqueadora"/>.</summary>
public class FranqueadoraConfiguration : IEntityTypeConfiguration<Franqueadora>
{
    public void Configure(EntityTypeBuilder<Franqueadora> builder)
    {
        builder.ToTable("Franqueadoras");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.RazaoSocial)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(f => f.NomeFantasia)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(f => f.Cnpj)
            .HasMaxLength(14)
            .IsFixedLength()
            .IsRequired();

        builder.Property(f => f.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(f => f.Telefone)
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(f => f.Cnpj)
            .IsUnique();

        builder.OwnsOne(f => f.Endereco, endereco => endereco.ConfigurarEndereco());

        builder.Navigation(f => f.Endereco).IsRequired();
    }
}
