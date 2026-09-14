using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Vendas;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IVendaService"/>.
/// </summary>
public sealed class VendaService(
    IVendaRepositorio vendas,
    IUnidadeRepositorio unidades,
    IProdutoServicoRepositorio produtos) : IVendaService
{
    /// <inheritdoc />
    public async Task<PagedResult<VendaResponse>> ListarAsync(
        QueryParams parametros,
        FiltroVendasRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var pagina = await vendas.ListarAsync(parametros, filtro, cancellationToken);

        return pagina.Converter(VendaResponse.De);
    }

    /// <inheritdoc />
    public async Task<VendaResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var venda = await BuscarOuFalharAsync(id, cancellationToken);

        return VendaResponse.De(venda);
    }

    /// <inheritdoc />
    public async Task<VendaResponse> RegistrarAsync(
        CriarVendaRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        // O DTO já exige ao menos um item, mas a entidade só recusa venda vazia na
        // confirmação. Sem esta checagem, uma chamada interna criaria uma venda pendente
        // sem nada dentro.
        if (requisicao.Itens.Count == 0)
        {
            throw new RegraDeNegocioException("A venda precisa de pelo menos um item.");
        }

        var unidade = await unidades.ObterPorIdAsync(requisicao.UnidadeFranqueadaId, cancellationToken)
            ?? throw new NaoEncontradoException("Unidade franqueada", requisicao.UnidadeFranqueadaId);

        var catalogo = await CarregarItensVendaveisAsync(requisicao.Itens, cancellationToken);

        var venda = new Venda(unidade.Id);

        foreach (var item in requisicao.Itens)
        {
            var produto = catalogo[item.ProdutoServicoId];
            venda.AdicionarItem(produto.Id, item.Quantidade, produto.PrecoBase);
        }

        await vendas.AdicionarAsync(venda, cancellationToken);
        await vendas.SalvarAlteracoesAsync(cancellationToken);

        return VendaResponse.De(await BuscarOuFalharAsync(venda.Id, cancellationToken));
    }

    /// <summary>
    /// Carrega de uma vez todos os itens do catálogo citados na venda e confere que cada um
    /// existe e pode ser vendido. A validação acontece inteira antes de a venda ser montada,
    /// de modo que um item inválido no fim da lista não deixa nada gravado.
    /// </summary>
    private async Task<Dictionary<int, ProdutoServico>> CarregarItensVendaveisAsync(
        IReadOnlyCollection<ItemVendaRequest> itens,
        CancellationToken cancellationToken)
    {
        var identificadores = itens.Select(item => item.ProdutoServicoId).Distinct().ToList();

        var catalogo = await produtos
            .Consultar()
            .Where(produto => identificadores.Contains(produto.Id))
            .ToDictionaryAsync(produto => produto.Id, cancellationToken);

        foreach (var identificador in identificadores)
        {
            if (!catalogo.TryGetValue(identificador, out var produto))
            {
                throw new NaoEncontradoException("Produto ou serviço", identificador);
            }

            if (!produto.EstaDisponivelParaVenda())
            {
                throw new RegraDeNegocioException(
                    $"O item '{produto.Nome}' não está disponível para venda.");
            }
        }

        return catalogo;
    }

    private async Task<Venda> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await vendas.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Venda", id);
}
