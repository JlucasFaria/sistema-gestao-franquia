using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Estoques;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
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

    /// <inheritdoc />
    public async Task<EstoqueResponse> RegistrarEntradaAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        MovimentacaoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var (unidade, produto) = await ExigirUnidadeEItemAptosAsync(
            unidadeFranqueadaId,
            produtoServicoId,
            cancellationToken);

        if (!produto.Ativo || produto.Status == StatusProduto.Descontinuado)
        {
            throw new RegraDeNegocioException(
                $"O item '{produto.Nome}' está descontinuado ou inativo e não recebe entrada de "
                + "estoque. A saída continua permitida, para escoar o saldo remanescente.");
        }

        var estoque = await ObterOuAbrirControleAsync(unidade, produto, cancellationToken);

        estoque.RegistrarEntrada(requisicao.Quantidade, requisicao.Observacao);
        await estoques.SalvarAlteracoesAsync(cancellationToken);

        return await ProjetarSaldoAsync(unidade.Id, produto.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EstoqueResponse> RegistrarSaidaAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        MovimentacaoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var (unidade, produto) = await ExigirUnidadeEItemAptosAsync(
            unidadeFranqueadaId,
            produtoServicoId,
            cancellationToken);

        var estoque = await BuscarSaldoOuFalharAsync(unidade.Id, produto.Id, cancellationToken);

        // A entidade também recusa saldo negativo, mas com uma exceção técnica que viraria
        // erro 500. A checagem aqui devolve ao cliente o que ele precisa saber: quanto há e
        // quanto foi pedido.
        if (!estoque.PossuiSaldoPara(requisicao.Quantidade))
        {
            throw new RegraDeNegocioException(
                $"Saldo insuficiente de '{produto.Nome}' na unidade '{unidade.NomeFantasia}': "
                + $"disponível {estoque.Quantidade}, solicitado {requisicao.Quantidade}.");
        }

        estoque.RegistrarSaida(requisicao.Quantidade, requisicao.Observacao);
        await estoques.SalvarAlteracoesAsync(cancellationToken);

        return await ProjetarSaldoAsync(unidade.Id, produto.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EstoqueResponse> AjustarAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        AjusteEstoqueRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var (unidade, produto) = await ExigirUnidadeEItemAptosAsync(
            unidadeFranqueadaId,
            produtoServicoId,
            cancellationToken);

        var estoque = await BuscarSaldoOuFalharAsync(unidade.Id, produto.Id, cancellationToken);

        estoque.Ajustar(requisicao.QuantidadeApurada, requisicao.Observacao);
        await estoques.SalvarAlteracoesAsync(cancellationToken);

        return await ProjetarSaldoAsync(unidade.Id, produto.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EstoqueResponse> DefinirQuantidadeMinimaAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        DefinirQuantidadeMinimaRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var (unidade, produto) = await ExigirUnidadeEItemAptosAsync(
            unidadeFranqueadaId,
            produtoServicoId,
            cancellationToken);

        // Definir o ponto de reposição antes da primeira compra é legítimo, então aqui o
        // controle também pode ser aberto.
        var estoque = await ObterOuAbrirControleAsync(unidade, produto, cancellationToken);

        estoque.DefinirQuantidadeMinima(requisicao.QuantidadeMinima);
        await estoques.SalvarAlteracoesAsync(cancellationToken);

        return await ProjetarSaldoAsync(unidade.Id, produto.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<MovimentacaoEstoqueResponse>> ListarMovimentacoesAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        QueryParams parametros,
        CancellationToken cancellationToken = default)
    {
        var estoque = await BuscarSaldoOuFalharAsync(
            unidadeFranqueadaId,
            produtoServicoId,
            cancellationToken);

        var pagina = await estoques.ListarMovimentacoesAsync(estoque.Id, parametros, cancellationToken);

        return pagina.Converter(MovimentacaoEstoqueResponse.De);
    }

    /// <summary>
    /// Confere a unidade e o item antes de qualquer movimentação, aplicando as regras comuns
    /// a entrada, saída e ajuste.
    /// </summary>
    private async Task<(UnidadeFranqueada Unidade, ProdutoServico Produto)> ExigirUnidadeEItemAptosAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        CancellationToken cancellationToken)
    {
        var unidade = await unidades.ObterPorIdAsync(unidadeFranqueadaId, cancellationToken)
            ?? throw new NaoEncontradoException("Unidade franqueada", unidadeFranqueadaId);

        // Unidade em implantação movimenta estoque: é assim que a loja é abastecida antes de
        // inaugurar. Só o encerramento do contrato e a inativação do cadastro impedem.
        if (!unidade.Ativo || unidade.Situacao == SituacaoUnidade.Encerrada)
        {
            throw new RegraDeNegocioException(
                $"A unidade '{unidade.NomeFantasia}' está encerrada ou inativa e não movimenta estoque.");
        }

        var produto = await produtos.ObterPorIdAsync(produtoServicoId, cancellationToken)
            ?? throw new NaoEncontradoException("Produto ou serviço", produtoServicoId);

        if (!produto.ControlaEstoque())
        {
            throw new RegraDeNegocioException(
                $"'{produto.Nome}' é um serviço e não movimenta estoque.");
        }

        return (unidade, produto);
    }

    private async Task<Estoque> ObterOuAbrirControleAsync(
        UnidadeFranqueada unidade,
        ProdutoServico produto,
        CancellationToken cancellationToken)
    {
        var estoque = await estoques.ObterPorUnidadeEProdutoAsync(
            unidade.Id,
            produto.Id,
            cancellationToken);

        if (estoque is not null)
        {
            return estoque;
        }

        var novo = new Estoque(unidade.Id, produto.Id, quantidadeMinima: 0);
        await estoques.AdicionarAsync(novo, cancellationToken);

        return novo;
    }

    /// <summary>
    /// Relê o saldo depois de gravar, para que a resposta traga unidade e item carregados
    /// mesmo quando o controle acabou de ser aberto.
    /// </summary>
    private async Task<EstoqueResponse> ProjetarSaldoAsync(
        int unidadeFranqueadaId,
        int produtoServicoId,
        CancellationToken cancellationToken)
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
