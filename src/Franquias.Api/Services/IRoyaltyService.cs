using Franquias.Api.Common.Consultas;
using Franquias.Api.DTOs.Royalties;

namespace Franquias.Api.Services;

/// <summary>
/// Apuração e gestão das cobranças de royalty das unidades franqueadas.
/// </summary>
public interface IRoyaltyService
{
    /// <summary>
    /// Lista as cobranças de royalty de forma paginada, com filtros por unidade e situação.
    /// As cobranças vencidas são marcadas como atrasadas antes da consulta.
    /// </summary>
    Task<PagedResult<RoyaltyResponse>> ListarAsync(
        QueryParams parametros,
        FiltroRoyaltiesRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma cobrança de royalty pelo identificador, já refletindo o atraso caso tenha
    /// vencido sem pagamento.
    /// </summary>
    Task<RoyaltyResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera a cobrança de royalty de uma unidade em um período já encerrado, calculada sobre
    /// o faturamento confirmado no período e o percentual vigente na unidade.
    /// </summary>
    Task<RoyaltyResponse> GerarAsync(
        GerarRoyaltyRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Dá baixa no pagamento de uma cobrança em aberto, pendente ou atrasada. O valor pago
    /// precisa cobrir o valor devido.
    /// </summary>
    Task<RoyaltyResponse> RegistrarPagamentoAsync(
        int id,
        RegistrarPagamentoRequest requisicao,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca como atrasadas as cobranças pendentes cujo vencimento é anterior à data de
    /// referência. Operação idempotente: repetir não produz efeito, e cobranças pagas nunca
    /// são afetadas.
    /// </summary>
    /// <param name="dataReferencia">Data considerada como hoje na avaliação do vencimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Quantidade de cobranças que passaram a atrasadas.</returns>
    Task<int> AtualizarAtrasosAsync(
        DateOnly dataReferencia,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resume os valores devidos, pagos e em aberto de uma unidade. A unidade sem cobranças
    /// no recorte recebe um resumo zerado.
    /// </summary>
    Task<ResumoRoyaltiesUnidadeResponse> ResumirUnidadeAsync(
        int unidadeFranqueadaId,
        FiltroResumoRoyaltiesRequest filtro,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resume os valores devidos, pagos e em aberto de cada unidade da rede que tenha
    /// cobranças no recorte, da maior para a menor dívida em aberto.
    /// </summary>
    Task<IReadOnlyList<ResumoRoyaltiesUnidadeResponse>> ResumirPorUnidadeAsync(
        FiltroResumoRoyaltiesRequest filtro,
        CancellationToken cancellationToken = default);
}
