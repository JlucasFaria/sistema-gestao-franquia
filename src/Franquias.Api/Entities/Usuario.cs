namespace Franquias.Api.Entities;

/// <summary>
/// Usuário que acessa a API. A senha nunca é armazenada em texto puro: a entidade guarda
/// apenas o hash produzido pelo serviço de autenticação.
/// </summary>
public class Usuario : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Usuario()
    {
    }

    public Usuario(string nome, string email, string senhaHash, int perfilId)
    {
        Nome = nome;
        Email = NormalizarEmail(email);
        SenhaHash = senhaHash;
        PerfilId = perfilId;
    }

    /// <summary>Nome completo do usuário.</summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>E-mail usado como credencial de login. Único em toda a base.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Hash BCrypt da senha.</summary>
    public string SenhaHash { get; private set; } = string.Empty;

    /// <summary>Chave estrangeira do perfil de acesso.</summary>
    public int PerfilId { get; private set; }

    /// <summary>Perfil de acesso do usuário.</summary>
    public Perfil Perfil { get; private set; } = null!;

    /// <summary>
    /// Altera nome e e-mail do usuário, registrando a data da alteração.
    /// </summary>
    public void AtualizarDados(string nome, string email)
    {
        Nome = nome;
        Email = NormalizarEmail(email);
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Substitui o hash da senha. O cálculo do hash é responsabilidade do serviço de autenticação.
    /// </summary>
    public void AlterarSenha(string senhaHash)
    {
        SenhaHash = senhaHash;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Vincula o usuário a outro perfil de acesso.
    /// </summary>
    public void VincularPerfil(int perfilId)
    {
        PerfilId = perfilId;
        RegistrarAtualizacao();
    }

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
