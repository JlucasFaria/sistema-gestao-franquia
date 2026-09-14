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

        ExigirUnidadeAptaAVender(unidade);

        var catalogo = await CarregarItensVendaveisAsync(requisicao.Itens, cancellationToken);

        var venda = new Venda(unidade.Id);

        // O total nunca vem da requisição: cada linha entra pela entidade, que recalcula
        // subtotais e total a cada item adicionado.
        foreach (var linha in ConsolidarItens(requisicao.Itens, catalogo))
        {
            venda.AdicionarItem(linha.ProdutoServicoId, linha.Quantidade, linha.PrecoUnitario);
        }

        await vendas.AdicionarAsync(venda, cancellationToken);
        await vendas.SalvarAlteracoesAsync(cancellationToken);

        return VendaResponse.De(await BuscarOuFalharAsync(venda.Id, cancellationToken));
    }

    /// <summary>
    /// Impede venda em unidade que não pode operar. A condição é a mesma de
    /// <see cref="UnidadeFranqueada.PodeOperar"/>: cadastro ativo e situação Ativa ao mesmo
    /// tempo. Unidade em implantação, suspensa ou encerrada não vende, e unidade inativada
    /// também não, mesmo que a situação contratual ainda conste como Ativa.
    /// </summary>
    private static void ExigirUnidadeAptaAVender(UnidadeFranqueada unidade)
    {
        if (unidade.PodeOperar())
        {
            return;
        }

        var motivo = unidade.Ativo
            ? $"está com a situação {unidade.Situacao}"
            : "está com o cadastro inativo";

        throw new RegraDeNegocioException(
            $"A unidade '{unidade.NomeFantasia}' não pode registrar vendas porque {motivo}. "
            + "Só vendem unidades com situação Ativa e cadastro ativo.");
    }

    /// <summary>
    /// Resolve o preço de cada item e junta as linhas repetidas. O preço é o praticado pela
    /// unidade quando informado e o de tabela quando omitido. Linhas do mesmo item com o
    /// mesmo preço viram uma só, com as quantidades somadas. Com preços diferentes elas
    /// continuam separadas: juntá-las obrigaria a escolher um dos preços e distorceria o total.
    /// </summary>
    private static IEnumerable<LinhaDeVenda> ConsolidarItens(
        IReadOnlyCollection<ItemVendaRequest> itens,
        IReadOnlyDictionary<int, ProdutoServico> catalogo) =>
        itens
            .Select(item => new LinhaDeVenda(
                item.ProdutoServicoId,
                item.Quantidade,
                item.PrecoUnitario ?? catalogo[item.ProdutoServicoId].PrecoBase))
            .GroupBy(linha => (linha.ProdutoServicoId, linha.PrecoUnitario))
            .Select(grupo => new LinhaDeVenda(
                grupo.Key.ProdutoServicoId,
                grupo.Sum(linha => linha.Quantidade),
                grupo.Key.PrecoUnitario));

    /// <summary>Linha já resolvida: item, quantidade total e preço efetivo.</summary>
    private sealed record LinhaDeVenda(int ProdutoServicoId, int Quantidade, decimal PrecoUnitario);

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
