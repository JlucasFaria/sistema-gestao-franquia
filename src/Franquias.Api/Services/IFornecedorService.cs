using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Fornecedores;

namespace Franquias.Api.Services;

/// <summary>
/// Regras de negócio dos fornecedores homologados pela rede.
/// </summary>
public interface IFornecedorService
{
    /// <summary>Lista os fornecedores de forma paginada.</summary>
    Task<PagedResult<FornecedorResponse>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default);

    /// <summary>Busca um fornecedor pelo identificador.</summary>
    Task<FornecedorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Cadastra um fornecedor, recusando CNPJ já em uso.</summary>
    Task<FornecedorResponse> CriarAsync(
        CriarFornecedorRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Altera os dados cadastrais de um fornecedor. O CNPJ é imutável.</summary>
    Task<FornecedorResponse> AtualizarAsync(
        int id,
        AtualizarFornecedorRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Reativa um fornecedor. Operação idempotente.</summary>
    Task<FornecedorResponse> AtivarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inativa um fornecedor: ele deixa de receber novas homologações, mas as existentes e o
    /// histórico de condições negociadas são preservados. Operação idempotente.
    /// </summary>
    Task<FornecedorResponse> InativarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui um fornecedor. Só é permitido enquanto ele não tiver produtos homologados.
    /// </summary>
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
