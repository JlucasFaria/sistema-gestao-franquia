using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="ChamadoSuporte"/>.</summary>
public class ChamadoSuporteConfiguration : IEntityTypeConfiguration<ChamadoSuporte>
{
    public void Configure(EntityTypeBuilder<ChamadoSuporte> builder)
    {
        builder.ToTable("Chamados");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Categoria)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.Assunto)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.Prioridade)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.HasOne(c => c.UnidadeFranqueada)
            .WithMany()
            .HasForeignKey(c => c.UnidadeFranqueadaId);

        builder.HasOne(c => c.UsuarioAbertura)
            .WithMany()
            .HasForeignKey(c => c.UsuarioAberturaId);

        builder.HasMany(c => c.Interacoes)
            .WithOne(i => i.ChamadoSuporte)
            .HasForeignKey(i => i.ChamadoSuporteId);

        builder.Navigation(c => c.Interacoes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Sustenta a consulta de chamados em aberto por prioridade e unidade.
        builder.HasIndex(c => new { c.UnidadeFranqueadaId, c.Status, c.Prioridade });
    }
}
