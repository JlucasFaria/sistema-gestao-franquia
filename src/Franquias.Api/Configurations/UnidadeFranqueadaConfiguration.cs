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

        // Percentual entre 0,00 e 999,99: três dígitos inteiros seriam desperdício, dois bastam.
        builder.Property(u => u.PercentualRoyalty)
            .HasPrecision(5, 2)
            .IsRequired();

        // Duas unidades não podem compartilhar o mesmo CNPJ.
        builder.HasIndex(u => u.Cnpj)
            .IsUnique();

        builder.OwnsOne(u => u.Endereco, endereco => endereco.ConfigurarEndereco());

        builder.Navigation(u => u.Endereco).IsRequired();

        builder.HasOne(u => u.Franqueadora)
            .WithMany()
            .HasForeignKey(u => u.FranqueadoraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Franqueado)
            .WithMany()
            .HasForeignKey(u => u.FranqueadoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Responsável só existe no contexto da unidade: acompanha a exclusão dela.
        builder.HasMany(u => u.Responsaveis)
            .WithOne(r => r.UnidadeFranqueada)
            .HasForeignKey(r => r.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Cascade);

        // A coleção é exposta como somente leitura; o EF escreve direto no campo de apoio.
        builder.Navigation(u => u.Responsaveis)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
