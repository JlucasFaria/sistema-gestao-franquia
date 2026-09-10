namespace Franquias.Api.Services;

/// <summary>
/// Geração e conferência de hashes de senha. Isola o algoritmo do restante da aplicação:
/// nenhum serviço ou controller conhece BCrypt diretamente.
/// </summary>
public interface IHashDeSenhaService
{
    /// <summary>Produz o hash da senha informada, com salt próprio.</summary>
    string GerarHash(string senha);

    /// <summary>
    /// Confere a senha contra o hash armazenado. Devolve falso — nunca lança — quando o
    /// hash está ausente ou corrompido.
    /// </summary>
    bool Verificar(string senha, string hashArmazenado);
}
