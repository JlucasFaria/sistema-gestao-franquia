using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="UnidadeFranqueada"/>.</summary>
public class UnidadeFranqueadaConfiguration : IEntityTypeConfiguration<UnidadeFranqueada>
{
    public void Configure(EntityTypeBuilder<UnidadeFranqueada> builder)
    {
        builder.ToTable("Unidades");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.RazaoSocial)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(u => u.NomeFantasia)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.Cnpj)
            .HasMaxLength(14)
            .IsFixedLength()
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(u => u.Telefone)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(u => u.DataInicio)
            .IsRequired();

        builder.Property(u => u.Situacao)
            .IsRequired();

        // Duas unidades não podem compartilhar o mesmo CNPJ.
        builder.HasIndex(u => u.Cnpj)
            .IsUnique();

        builder.OwnsOne(u => u.Endereco, endereco => endereco.ConfigurarEndereco());

        builder.Navigation(u => u.Endereco).IsRequired();

        builder.HasOne(u => u.Franqueadora)
            .WithMany()
            .HasForeignKey(u => u.FranqueadoraId);

        builder.HasOne(u => u.Franqueado)
            .WithMany()
            .HasForeignKey(u => u.FranqueadoId);

        builder.HasMany(u => u.Responsaveis)
            .WithOne(r => r.UnidadeFranqueada)
            .HasForeignKey(r => r.UnidadeFranqueadaId);

        // A coleção é exposta como somente leitura; o EF escreve direto no campo de apoio.
        builder.Navigation(u => u.Responsaveis)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
