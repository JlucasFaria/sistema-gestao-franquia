using Franquias.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento do objeto de valor <see cref="Endereco"/>. Como ele é compartilhado por
/// franqueadora, unidade e fornecedor, a configuração fica em um único lugar e é aplicada
/// por cada dono.
/// </summary>
public static class EnderecoConfiguration
{
    /// <summary>
    /// Aplica o mapeamento das colunas de endereço na tabela da entidade dona.
    /// </summary>
    public static void ConfigurarEndereco<TDono>(this OwnedNavigationBuilder<TDono, Endereco> endereco)
        where TDono : class
    {
        endereco.Property(e => e.Logradouro)
            .HasColumnName("Logradouro")
            .HasMaxLength(180)
            .IsRequired();

        endereco.Property(e => e.Numero)
            .HasColumnName("Numero")
            .HasMaxLength(10)
            .IsRequired();

        endereco.Property(e => e.Complemento)
            .HasColumnName("Complemento")
            .HasMaxLength(60);

        endereco.Property(e => e.Bairro)
            .HasColumnName("Bairro")
            .HasMaxLength(100)
            .IsRequired();

        endereco.Property(e => e.Cidade)
            .HasColumnName("Cidade")
            .HasMaxLength(100)
            .IsRequired();

        endereco.Property(e => e.Uf)
            .HasColumnName("Uf")
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();

        endereco.Property(e => e.Cep)
            .HasColumnName("Cep")
            .HasMaxLength(8)
            .IsFixedLength()
            .IsRequired();
    }
}
