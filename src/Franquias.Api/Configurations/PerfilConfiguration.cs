using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Perfil"/>.</summary>
public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> builder)
    {
        builder.ToTable("Perfis");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Codigo)
            .IsRequired();

        builder.Property(p => p.Nome)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasMaxLength(200);

        // Cada valor de PerfilAcesso corresponde a exatamente um registro de perfil.
        builder.HasIndex(p => p.Codigo)
            .IsUnique();
    }
}
