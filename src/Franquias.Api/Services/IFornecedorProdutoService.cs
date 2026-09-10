using Franquias.Api.DTOs.Fornecedores;

namespace Franquias.Api.Services;

/// <summary>
/// Homologação de fornecedores para itens do catálogo, com as condições comerciais de cada par.
/// </summary>
public interface IFornecedorProdutoService
{
    /// <summary>Lista os itens homologados para um fornecedor.</summary>
    Task<IReadOnlyCollection<FornecedorProdutoResponse>> ListarAsync(
        int fornecedorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Homologa o fornecedor para um item do catálogo, recusando par já homologado.
    /// </summary>
    Task<FornecedorProdutoResponse> AssociarAsync(
        int fornecedorId,
        AssociarProdutoRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Renegocia preço e prazo de um item já homologado para o fornecedor.</summary>
    Task<FornecedorProdutoResponse> AtualizarCondicoesAsync(
        int fornecedorId,
        int produtoServicoId,
        AtualizarCondicoesRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>Remove a homologação do fornecedor para um item.</summary>
    Task DesassociarAsync(
        int fornecedorId,
        int produtoServicoId,
        CancellationToken cancellationToken = default);
}
