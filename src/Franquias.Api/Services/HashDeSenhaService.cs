namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IHashDeSenhaService"/> sobre o BCrypt.
/// </summary>
public sealed class HashDeSenhaService : IHashDeSenhaService
{
    /// <summary>
    /// Custo do algoritmo. Cada incremento dobra o tempo de cálculo, encarecendo ataques
    /// de força bruta. O valor fica gravado dentro do próprio hash, de modo que elevá-lo
    /// no futuro não invalida as senhas já cadastradas.
    /// </summary>
    private const int FatorDeTrabalho = 11;

    /// <inheritdoc />
    public string GerarHash(string senha) =>
        BCrypt.Net.BCrypt.HashPassword(senha, FatorDeTrabalho);

    /// <inheritdoc />
    public bool Verificar(string senha, string hashArmazenado)
    {
        if (string.IsNullOrWhiteSpace(hashArmazenado))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hashArmazenado);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Hash malformado no banco é dado inválido, não falha do servidor: recusar a
            // credencial é o correto, e evita transformar o login em erro 500.
            return false;
        }
    }
}
