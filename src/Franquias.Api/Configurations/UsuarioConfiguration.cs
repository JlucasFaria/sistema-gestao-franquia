using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>Mapeamento da entidade <see cref="Usuario"/>.</summary>
public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(180)
            .IsRequired();

        // O hash BCrypt tem 60 caracteres; a folga acomoda uma eventual troca de algoritmo.
        builder.Property(u => u.SenhaHash)
            .HasMaxLength(100)
            .IsRequired();

        // Credencial de login: garante no banco a regra de e-mail único do UsuarioService.
        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Perfil em uso não pode ser apagado enquanto houver usuário vinculado.
        builder.HasOne(u => u.Perfil)
            .WithMany()
            .HasForeignKey(u => u.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
