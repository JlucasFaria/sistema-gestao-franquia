using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Fornecedores;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IFornecedorProdutoService"/>. Toda alteração passa pelo
/// agregado <see cref="Fornecedor"/>, dono da coleção de homologações.
/// </summary>
public sealed class FornecedorProdutoService(
    IFornecedorRepositorio fornecedores,
    IProdutoServicoRepositorio produtos) : IFornecedorProdutoService
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<FornecedorProdutoResponse>> ListarAsync(
        int fornecedorId,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarFornecedorOuFalharAsync(fornecedorId, cancellationToken);

        return
        [
            .. fornecedor.Produtos
                .OrderBy(vinculo => vinculo.ProdutoServico.Nome)
                .Select(FornecedorProdutoResponse.De)
        ];
    }

    /// <inheritdoc />
    public async Task<FornecedorProdutoResponse> AssociarAsync(
        int fornecedorId,
        AssociarProdutoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarFornecedorOuFalharAsync(fornecedorId, cancellationToken);

        if (!fornecedor.Ativo)
        {
            throw new RegraDeNegocioException(
                $"O fornecedor '{fornecedor.NomeFantasia}' está inativo e não recebe novas homologações.");
        }

        var produto = await produtos.ObterPorIdAsync(requisicao.ProdutoServicoId, cancellationToken)
            ?? throw new NaoEncontradoException("Produto ou serviço", requisicao.ProdutoServicoId);

        // Item com status Inativo é aceito: está só temporariamente fora de venda, e a rede
        // pode querer um fornecedor pronto para quando voltar. Descontinuado ou com cadastro
        // inativo não volta, então homologá-lo não teria propósito.
        if (!produto.Ativo || produto.Status == StatusProduto.Descontinuado)
        {
            throw new RegraDeNegocioException(
                $"O item '{produto.Nome}' está descontinuado ou inativo e não pode ser homologado.");
        }

        if (fornecedor.Produtos.Any(vinculo => vinculo.ProdutoServicoId == produto.Id))
        {
            throw new ConflitoException(
                $"O fornecedor '{fornecedor.NomeFantasia}' já está homologado para '{produto.Nome}'. "
                + "Para mudar preço ou prazo, atualize as condições do vínculo existente.");
        }

        var vinculo = new FornecedorProduto(
            fornecedor.Id,
            produto.Id,
            requisicao.PrecoFornecimento,
            requisicao.PrazoEntregaEmDias);

        fornecedor.AssociarProduto(vinculo);
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorProdutoResponse.De(vinculo);
    }

    /// <inheritdoc />
    public async Task<FornecedorProdutoResponse> AtualizarCondicoesAsync(
        int fornecedorId,
        int produtoServicoId,
        AtualizarCondicoesRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarFornecedorOuFalharAsync(fornecedorId, cancellationToken);
        var vinculo = BuscarVinculoOuFalhar(fornecedor, produtoServicoId);

        vinculo.AtualizarCondicoes(requisicao.PrecoFornecimento, requisicao.PrazoEntregaEmDias);
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);

        return FornecedorProdutoResponse.De(vinculo);
    }

    /// <inheritdoc />
    public async Task DesassociarAsync(
        int fornecedorId,
        int produtoServicoId,
        CancellationToken cancellationToken = default)
    {
        var fornecedor = await BuscarFornecedorOuFalharAsync(fornecedorId, cancellationToken);
        var vinculo = BuscarVinculoOuFalhar(fornecedor, produtoServicoId);

        // A chave estrangeira é obrigatória: retirar o vínculo da coleção faz o EF excluí-lo.
        fornecedor.DesassociarProduto(vinculo);
        await fornecedores.SalvarAlteracoesAsync(cancellationToken);
    }

    private async Task<Fornecedor> BuscarFornecedorOuFalharAsync(
        int fornecedorId,
        CancellationToken cancellationToken) =>
        await fornecedores.ObterComProdutosAsync(fornecedorId, cancellationToken)
            ?? throw new NaoEncontradoException("Fornecedor", fornecedorId);

    /// <summary>
    /// Procura o vínculo dentro das homologações do fornecedor informado na rota. Um item
    /// homologado apenas para outro fornecedor é tratado como inexistente aqui.
    /// </summary>
    private static FornecedorProduto BuscarVinculoOuFalhar(Fornecedor fornecedor, int produtoServicoId) =>
        fornecedor.Produtos.FirstOrDefault(vinculo => vinculo.ProdutoServicoId == produtoServicoId)
            ?? throw new NaoEncontradoException(
                $"O item {produtoServicoId} não está homologado para o fornecedor {fornecedor.Id}.");
}
