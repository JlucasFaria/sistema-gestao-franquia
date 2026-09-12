using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Estoque"/>.</summary>
public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        // Última barreira contra saldo negativo. O serviço recusa a saída sem saldo e a
        // entidade também, mas nenhum dos dois alcança um UPDATE feito direto no banco;
        // a restrição vale para qualquer caminho de escrita.
        builder.ToTable("Estoques", tabela =>
        {
            tabela.HasCheckConstraint("CK_Estoques_QuantidadeNaoNegativa", @"""Quantidade"" >= 0");
            tabela.HasCheckConstraint(
                "CK_Estoques_QuantidadeMinimaNaoNegativa",
                @"""QuantidadeMinima"" >= 0");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Quantidade)
            .IsRequired();

        builder.Property(e => e.QuantidadeMinima)
            .IsRequired();

        builder.HasOne(e => e.UnidadeFranqueada)
            .WithMany()
            .HasForeignKey(e => e.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ProdutoServico)
            .WithMany()
            .HasForeignKey(e => e.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        // O histórico não sobrevive ao saldo que o originou.
        builder.HasMany(e => e.Movimentacoes)
            .WithOne(m => m.Estoque)
            .HasForeignKey(m => m.EstoqueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Um único saldo por par unidade/item: impede dois registros disputando o mesmo estoque.
        builder.HasIndex(e => new { e.UnidadeFranqueadaId, e.ProdutoServicoId })
            .IsUnique();

        builder.Navigation(e => e.Movimentacoes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
