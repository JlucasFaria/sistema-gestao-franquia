using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Usuarios;

/// <summary>
/// Dados de um usuário devolvidos pela API. O hash da senha nunca é exposto.
/// </summary>
/// <param name="Id">Identificador do usuário.</param>
/// <param name="Nome">Nome completo.</param>
/// <param name="Email">E-mail de login.</param>
/// <param name="PerfilId">Identificador do perfil de acesso.</param>
/// <param name="Perfil">Nome do perfil de acesso.</param>
/// <param name="Ativo">Indica se o usuário pode autenticar.</param>
/// <param name="DataCriacao">Momento do cadastro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
public sealed record UsuarioResponse(
    int Id,
    string Nome,
    string Email,
    int PerfilId,
    string Perfil,
    bool Ativo,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>
    /// Projeta a entidade no DTO de saída. Exige a navegação de perfil carregada.
    /// </summary>
    public static UsuarioResponse De(Usuario usuario) => new(
        usuario.Id,
        usuario.Nome,
        usuario.Email,
        usuario.PerfilId,
        usuario.Perfil?.Nome ?? string.Empty,
        usuario.Ativo,
        usuario.DataCriacao,
        usuario.DataAtualizacao);
}
