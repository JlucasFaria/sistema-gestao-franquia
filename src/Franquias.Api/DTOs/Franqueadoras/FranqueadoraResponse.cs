using Franquias.Api.DTOs.Enderecos;
using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Franqueadoras;

/// <summary>
/// Dados da franqueadora devolvidos pela API.
/// </summary>
/// <param name="Id">Identificador da franqueadora.</param>
/// <param name="RazaoSocial">Razão social registrada.</param>
/// <param name="NomeFantasia">Nome comercial da marca.</param>
/// <param name="Cnpj">CNPJ, apenas dígitos.</param>
/// <param name="Email">E-mail de contato da matriz.</param>
/// <param name="Telefone">Telefone de contato, apenas dígitos.</param>
/// <param name="Endereco">Endereço da matriz.</param>
/// <param name="Ativo">Indica se a franqueadora está ativa.</param>
/// <param name="DataCriacao">Momento do cadastro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
public sealed record FranqueadoraResponse(
    int Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string Email,
    string Telefone,
    EnderecoDto Endereco,
    bool Ativo,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>Projeta a entidade no DTO de saída.</summary>
    public static FranqueadoraResponse De(Franqueadora franqueadora) => new(
        franqueadora.Id,
        franqueadora.RazaoSocial,
        franqueadora.NomeFantasia,
        franqueadora.Cnpj,
        franqueadora.Email,
        franqueadora.Telefone,
        EnderecoDto.De(franqueadora.Endereco),
        franqueadora.Ativo,
        franqueadora.DataCriacao,
        franqueadora.DataAtualizacao);
}
