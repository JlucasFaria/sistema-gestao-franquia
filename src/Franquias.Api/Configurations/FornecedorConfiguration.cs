using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Fornecedor"/>.</summary>
public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.ToTable("Fornecedores");

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

        builder.OwnsOne(f => f.Endereco, endereco => endereco.ConfigurarEndereco());

        builder.Navigation(f => f.Endereco).IsRequired();

        builder.HasMany(f => f.Produtos)
            .WithOne(fp => fp.Fornecedor)
            .HasForeignKey(fp => fp.FornecedorId);

        builder.Navigation(f => f.Produtos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
