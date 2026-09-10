using Franquias.Api.DTOs.Enderecos;
using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Fornecedores;

/// <summary>
/// Dados de um fornecedor devolvidos pela API.
/// </summary>
/// <param name="Id">Identificador do fornecedor.</param>
/// <param name="RazaoSocial">Razão social registrada.</param>
/// <param name="NomeFantasia">Nome comercial.</param>
/// <param name="Cnpj">CNPJ, apenas dígitos.</param>
/// <param name="Email">E-mail de contato.</param>
/// <param name="Telefone">Telefone de contato, apenas dígitos.</param>
/// <param name="Endereco">Endereço do fornecedor.</param>
/// <param name="Ativo">Indica se o fornecedor está ativo e pode receber novas homologações.</param>
/// <param name="DataCriacao">Momento do cadastro, em UTC.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
public sealed record FornecedorResponse(
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
    public static FornecedorResponse De(Fornecedor fornecedor) => new(
        fornecedor.Id,
        fornecedor.RazaoSocial,
        fornecedor.NomeFantasia,
        fornecedor.Cnpj,
        fornecedor.Email,
        fornecedor.Telefone,
        EnderecoDto.De(fornecedor.Endereco),
        fornecedor.Ativo,
        fornecedor.DataCriacao,
        fornecedor.DataAtualizacao);
}
