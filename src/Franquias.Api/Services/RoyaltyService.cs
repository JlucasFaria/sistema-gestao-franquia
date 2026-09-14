using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IRoyaltyService"/>.
/// </summary>
public sealed class RoyaltyService(
    IRoyaltyRepositorio royalties,
    IUnidadeRepositorio unidades,
    IVendaRepositorio vendas) : IRoyaltyService
{
    /// <inheritdoc />
    public async Task<RoyaltyResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var royalty = await BuscarOuFalharAsync(id, cancellationToken);

        return RoyaltyResponse.De(royalty);
    }

    /// <inheritdoc />
    public async Task<RoyaltyResponse> GerarAsync(
        GerarRoyaltyRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var periodoInicio = requisicao.PeriodoInicio!.Value;
        var periodoFim = requisicao.PeriodoFim!.Value;
        var dataVencimento = requisicao.DataVencimento!.Value;

        ExigirDatasCoerentes(periodoInicio, periodoFim, dataVencimento);

        var unidade = await unidades.ObterPorIdAsync(requisicao.UnidadeFranqueadaId, cancellationToken)
            ?? throw new NaoEncontradoException("Unidade franqueada", requisicao.UnidadeFranqueadaId);

        if (await royalties.ExisteSobreposicaoAsync(unidade.Id, periodoInicio, periodoFim, cancellationToken))
        {
            throw new ConflitoException(
                $"A unidade '{unidade.NomeFantasia}' já tem cobrança de royalty com período que se "
                + $"sobrepõe a {periodoInicio:dd/MM/yyyy}–{periodoFim:dd/MM/yyyy}. Um mesmo dia de "
                + "faturamento não pode ser cobrado duas vezes.");
        }

        var faturamento = await vendas.SomarFaturamentoConfirmadoAsync(
            unidade.Id,
            periodoInicio,
            periodoFim,
            cancellationToken);

        // O percentual é copiado para a cobrança: uma renegociação futura do contrato não
        // pode alterar o valor de uma cobrança já emitida.
        var royalty = new Royalty(
            unidade.Id,
            periodoInicio,
            periodoFim,
            faturamento,
            unidade.PercentualRoyalty,
            dataVencimento);

        // Uma cobrança de valor zero nunca poderia ser quitada, já que o pagamento exige valor
        // positivo: ficaria pendente para sempre e acabaria marcada como atrasada.
        if (royalty.ValorDevido == decimal.Zero)
        {
            throw new RegraDeNegocioException(
                $"Não há valor a cobrar da unidade '{unidade.NomeFantasia}' no período: faturamento "
                + $"confirmado de {faturamento:N2} com percentual de {unidade.PercentualRoyalty:N2}%.");
        }

        await royalties.AdicionarAsync(royalty, cancellationToken);
        await royalties.SalvarAlteracoesAsync(cancellationToken);

        return RoyaltyResponse.De(await BuscarOuFalharAsync(royalty.Id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<RoyaltyResponse> RegistrarPagamentoAsync(
        int id,
        RegistrarPagamentoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var royalty = await BuscarOuFalharAsync(id, cancellationToken);

        // A entidade também recusa quitar duas vezes, mas com uma exceção técnica. Aqui a
        // recusa vira resposta tratada, informando quando a quitação anterior aconteceu.
        if (!royalty.EstaEmAberto())
        {
            throw new RegraDeNegocioException(
                $"A cobrança {royalty.Id} já foi quitada em {royalty.DataPagamento:dd/MM/yyyy}.");
        }

        var dataPagamento = requisicao.DataPagamento!.Value;

        if (dataPagamento > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new RegraDeNegocioException("A data do pagamento não pode estar no futuro.");
        }

        // Pagamento parcial não quita: marcar a cobrança como paga esconderia o saldo que
        // continua devido. Valor maior é aceito, pois inclui multa e juros de um pagamento em atraso.
        if (requisicao.ValorPago < royalty.ValorDevido)
        {
            throw new RegraDeNegocioException(
                $"O valor pago ({requisicao.ValorPago:N2}) é menor que o valor devido "
                + $"({royalty.ValorDevido:N2}). Pagamento parcial não quita a cobrança.");
        }

        royalty.RegistrarPagamento(dataPagamento, requisicao.ValorPago);
        await royalties.SalvarAlteracoesAsync(cancellationToken);

        return RoyaltyResponse.De(royalty);
    }

    /// <inheritdoc />
    public async Task<int> AtualizarAtrasosAsync(
        DateOnly dataReferencia,
        CancellationToken cancellationToken = default)
    {
        // A consulta já traz só as pendentes vencidas; a própria entidade confirma a regra
        // de atraso antes de mudar a situação.
        var vencidas = await royalties.ListarPendentesVencidasAsync(dataReferencia, cancellationToken);

        foreach (var royalty in vencidas)
        {
            royalty.AvaliarAtraso(dataReferencia);
        }

        if (vencidas.Count > 0)
        {
            await royalties.SalvarAlteracoesAsync(cancellationToken);
        }

        return vencidas.Count;
    }

    /// <summary>
    /// Confere as datas da apuração. O DTO já valida a ordem entre elas; a checagem do
    /// período encerrado depende do relógio e só pode ser feita aqui.
    /// </summary>
    private static void ExigirDatasCoerentes(DateOnly periodoInicio, DateOnly periodoFim, DateOnly dataVencimento)
    {
        if (periodoFim < periodoInicio)
        {
            throw new RegraDeNegocioException("O fim do período não pode ser anterior ao início.");
        }

        if (dataVencimento <= periodoFim)
        {
            throw new RegraDeNegocioException("O vencimento precisa ser posterior ao fim do período de apuração.");
        }

        // Apurar um período que ainda não terminou cobraria sobre um faturamento incompleto:
        // as vendas dos dias restantes ficariam de fora e não poderiam mais ser cobradas.
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        if (periodoFim >= hoje)
        {
            throw new RegraDeNegocioException(
                $"O período termina em {periodoFim:dd/MM/yyyy} e ainda não foi encerrado. A cobrança "
                + "só pode ser gerada a partir do dia seguinte ao fim do período.");
        }
    }

    private async Task<Royalty> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await royalties.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Cobrança de royalty", id);
}
