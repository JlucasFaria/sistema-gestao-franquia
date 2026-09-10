using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Responsaveis;

/// <summary>
/// Responsável por uma unidade, devolvido pela API.
/// </summary>
/// <param name="Id">Identificador do responsável.</param>
/// <param name="UnidadeFranqueadaId">Unidade pela qual responde.</param>
/// <param name="Nome">Nome completo.</param>
/// <param name="Cpf">CPF, apenas dígitos.</param>
/// <param name="Cargo">Cargo exercido na unidade.</param>
/// <param name="Email">E-mail de contato.</param>
/// <param name="Telefone">Telefone de contato, apenas dígitos.</param>
public sealed record ResponsavelResponse(
    int Id,
    int UnidadeFranqueadaId,
    string Nome,
    string Cpf,
    string Cargo,
    string Email,
    string Telefone)
{
    /// <summary>Projeta a entidade no DTO de saída.</summary>
    public static ResponsavelResponse De(Responsavel responsavel) => new(
        responsavel.Id,
        responsavel.UnidadeFranqueadaId,
        responsavel.Nome,
        responsavel.Cpf,
        responsavel.Cargo,
        responsavel.Email,
        responsavel.Telefone);
}
