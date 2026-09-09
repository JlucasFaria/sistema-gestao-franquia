using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Responsavel"/>.</summary>
public class ResponsavelConfiguration : IEntityTypeConfiguration<Responsavel>
{
    public void Configure(EntityTypeBuilder<Responsavel> builder)
    {
        builder.ToTable("Responsaveis");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nome)
            .HasMaxLength(150)
            .IsRequired();

        // Sem índice único: a mesma pessoa pode responder por mais de uma unidade da rede.
        builder.Property(r => r.Cpf)
            .HasMaxLength(11)
            .IsFixedLength()
            .IsRequired();

        builder.Property(r => r.Cargo)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(r => r.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(r => r.Telefone)
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(r => r.UnidadeFranqueadaId);
    }
}
