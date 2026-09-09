using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="MovimentacaoEstoque"/>.</summary>
public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Tipo)
            .IsRequired();

        builder.Property(m => m.Quantidade)
            .IsRequired();

        builder.Property(m => m.SaldoAnterior)
            .IsRequired();

        builder.Property(m => m.SaldoResultante)
            .IsRequired();

        builder.Property(m => m.Observacao)
            .HasMaxLength(250);

        // Consulta típica do histórico: movimentações de um saldo, da mais recente para a mais antiga.
        builder.HasIndex(m => new { m.EstoqueId, m.DataCriacao });
    }
}
