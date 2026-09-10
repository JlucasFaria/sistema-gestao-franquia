using Franquias.Api.DTOs.Enderecos;
using Franquias.Api.DTOs.Responsaveis;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Unidades;

/// <summary>
/// Dados de uma unidade franqueada devolvidos pela API.
/// </summary>
/// <param name="Id">Identificador da unidade.</param>
/// <param name="FranqueadoraId">Franqueadora dona da marca.</param>
/// <param name="Franqueadora">Nome fantasia da franqueadora.</param>
/// <param name="FranqueadoId">Franqueado que opera a unidade.</param>
/// <param name="Franqueado">Nome do franqueado.</param>
/// <param name="RazaoSocial">Razão social da unidade.</param>
/// <param name="NomeFantasia">Nome pelo qual a unidade é conhecida.</param>
/// <param name="Cnpj">CNPJ, apenas dígitos.</param>
/// <param name="Email">E-mail de contato.</param>
/// <param name="Telefone">Telefone de contato, apenas dígitos.</param>
/// <param name="Endereco">Endereço da unidade.</param>
/// <param name="DataInicio">Data de início da operação.</param>
/// <param name="Situacao">Situação contratual.</param>
/// <param name="PercentualRoyalty">Percentual de royalty sobre o faturamento.</param>
/// <param name="Ativo">Indica se o cadastro está ativo.</param>
/// <param name="PodeOperar">Indica se a unidade está apta a registrar vendas e movimentar estoque.</param>
/// <param name="Responsaveis">Pessoas responsáveis pela operação.</param>
/// <param name="DataCriacao">Momento do cadastro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
public sealed record UnidadeResponse(
    int Id,
    int FranqueadoraId,
    string Franqueadora,
    int FranqueadoId,
    string Franqueado,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string Email,
    string Telefone,
    EnderecoDto Endereco,
    DateOnly DataInicio,
    SituacaoUnidade Situacao,
    decimal PercentualRoyalty,
    bool Ativo,
    bool PodeOperar,
    IReadOnlyCollection<ResponsavelResponse> Responsaveis,
    DateTime DataCriacao,
    DateTime? DataAtualizacao)
{
    /// <summary>
    /// Projeta a entidade no DTO de saída. Espera franqueadora, franqueado e responsáveis
    /// carregados.
    /// </summary>
    public static UnidadeResponse De(UnidadeFranqueada unidade) => new(
        unidade.Id,
        unidade.FranqueadoraId,
        unidade.Franqueadora?.NomeFantasia ?? string.Empty,
        unidade.FranqueadoId,
        unidade.Franqueado?.Nome ?? string.Empty,
        unidade.RazaoSocial,
        unidade.NomeFantasia,
        unidade.Cnpj,
        unidade.Email,
        unidade.Telefone,
        EnderecoDto.De(unidade.Endereco),
        unidade.DataInicio,
        unidade.Situacao,
        unidade.PercentualRoyalty,
        unidade.Ativo,
        unidade.PodeOperar(),
        [.. unidade.Responsaveis.OrderBy(responsavel => responsavel.Nome).Select(ResponsavelResponse.De)],
        unidade.DataCriacao,
        unidade.DataAtualizacao);
}
