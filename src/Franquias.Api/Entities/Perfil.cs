using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Perfil de acesso atribuído a um usuário. Cada perfil corresponde a um valor de
/// <see cref="PerfilAcesso"/>, que é o que viaja nas claims do token JWT.
/// </summary>
public class Perfil : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Perfil()
    {
    }

    public Perfil(PerfilAcesso codigo, string nome, string? descricao = null)
    {
        Codigo = codigo;
        Nome = nome;
        Descricao = descricao;
    }

    /// <summary>Código do perfil, usado nas políticas de autorização.</summary>
    public PerfilAcesso Codigo { get; private set; }

    /// <summary>Nome exibido do perfil.</summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>Descrição das permissões que o perfil concede.</summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Altera o nome e a descrição do perfil, registrando a data da alteração.
    /// </summary>
    public void Atualizar(string nome, string? descricao)
    {
        Nome = nome;
        Descricao = descricao;
        RegistrarAtualizacao();
    }
}
