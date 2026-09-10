using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Franqueados;

/// <summary>
/// Dados do franqueado devolvidos pela API.
/// </summary>
/// <param name="Id">Identificador do franqueado.</param>
/// <param name="Nome">Nome completo.</param>
/// <param name="Cpf">CPF, apenas dígitos.</param>
/// <param name="Email">E-mail de contato.</param>
/// <param name="Telefone">Telefone de contato, apenas dígitos.</param>
/// <param name="DataAdesao">Data de adesão à rede.</param>
/// <param name="Ativo">Indica se o franqueado está ativo.</param>
/// <param name="DataCriacao">Momento do cadastro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
public sealed record FranqueadoResponse(
    int Id,
    string Nome,
    string Cpf,
    string Email,
    string Telefone,
    DateOnly DataAdesao,
    bool Ativo,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>Projeta a entidade no DTO de saída.</summary>
    public static FranqueadoResponse De(Franqueado franqueado) => new(
        franqueado.Id,
        franqueado.Nome,
        franqueado.Cpf,
        franqueado.Email,
        franqueado.Telefone,
        franqueado.DataAdesao,
        franqueado.Ativo,
        franqueado.DataCriacao,
        franqueado.DataAtualizacao);
}
