using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Perfis;

/// <summary>
/// Perfil de acesso devolvido pela API.
/// </summary>
/// <param name="Id">Identificador do perfil.</param>
/// <param name="Codigo">Código do perfil, usado nas políticas de autorização.</param>
/// <param name="Nome">Nome exibido.</param>
/// <param name="Descricao">Descrição das permissões concedidas.</param>
/// <param name="Ativo">Indica se o perfil pode ser atribuído.</param>
public sealed record PerfilResponse(
    int Id,
    string Codigo,
    string Nome,
    string? Descricao,
    bool Ativo)
{
    /// <summary>Projeta a entidade no DTO de saída.</summary>
    public static PerfilResponse De(Perfil perfil) => new(
        perfil.Id,
        perfil.Codigo.ToString(),
        perfil.Nome,
        perfil.Descricao,
        perfil.Ativo);
}
