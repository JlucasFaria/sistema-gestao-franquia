using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="InteracaoChamado"/>.</summary>
public class InteracaoChamadoConfiguration : IEntityTypeConfiguration<InteracaoChamado>
{
    public void Configure(EntityTypeBuilder<InteracaoChamado> builder)
    {
        builder.ToTable("InteracoesChamado");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Mensagem)
            .HasMaxLength(2000)
            .IsRequired();

        // Preserva a autoria do histórico: usuário com interações registradas não pode ser apagado.
        builder.HasOne(i => i.Usuario)
            .WithMany()
            .HasForeignKey(i => i.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
