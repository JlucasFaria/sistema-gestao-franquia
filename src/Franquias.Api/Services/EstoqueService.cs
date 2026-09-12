using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Estoques;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IEstoqueService"/>.
/// </summary>
public sealed class EstoqueService(
    IEstoqueRepositorio estoques,
    IUnidadeRepositorio unidades,
    IProdutoServicoRepositorio produtos) : IEstoqueService
{
    /// <inheritdoc />
    public async Task<PagedResult<EstoqueResponse>> ListarAsync(
        QueryParams parametros,
        FiltroEstoquesRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var pagina = await estoques.ListarAsync(parametros, filtro, cancellationToken);

        return pagina.Converter(EstoqueResponse.De);
    }

    /// <inheritdoc />
    public async Task<EstoqueResponse> ObterSaldoAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        CancellationToken cancellationToken = default)
    {
        var estoque = await BuscarSaldoOuFalharAsync(
            unidadeFranqueadaId,
            produtoServicoId,
            cancellationToken);

        return EstoqueResponse.De(estoque);
    }

    /// <summary>
    /// Localiza o saldo conferindo antes a unidade e o item, para que um identificador
    /// errado produza uma mensagem precisa em vez de um genérico "estoque não encontrado".
    /// </summary>
    private async Task<Estoque> BuscarSaldoOuFalharAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.ObterPorIdAsync(unidadeFranqueadaId, cancellationToken)
            ?? throw new NaoEncontradoException("Unidade franqueada", unidadeFranqueadaId);

        var produto = await produtos.ObterPorIdAsync(produtoServicoId, cancellationToken)
            ?? throw new NaoEncontradoException("Produto ou serviço", produtoServicoId);

        return await estoques.ObterPorUnidadeEProdutoAsync(
                unidadeFranqueadaId,
                produtoServicoId,
                cancellationToken)
            ?? throw new NaoEncontradoException(
                $"Não há controle de estoque do item '{produto.Nome}' na unidade "
                + $"'{unidade.NomeFantasia}'. Registre uma entrada para iniciar o controle.");
    }
}
