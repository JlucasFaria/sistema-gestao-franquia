using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="FornecedorProduto"/>.</summary>
public class FornecedorProdutoConfiguration : IEntityTypeConfiguration<FornecedorProduto>
{
    public void Configure(EntityTypeBuilder<FornecedorProduto> builder)
    {
        builder.ToTable("FornecedoresProdutos");

        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.PrazoEntregaEmDias)
            .IsRequired();

        builder.Property(fp => fp.PrecoFornecimento)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(fp => fp.ProdutoServico)
            .WithMany()
            .HasForeignKey(fp => fp.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Um fornecedor é homologado uma única vez para cada item do catálogo.
        builder.HasIndex(fp => new { fp.FornecedorId, fp.ProdutoServicoId })
            .IsUnique();
    }
}
