using Franquias.Api.DTOs.Royalties;

namespace Franquias.Api.Services;

/// <summary>
/// Apuração e gestão das cobranças de royalty das unidades franqueadas.
/// </summary>
public interface IRoyaltyService
{
    /// <summary>Busca uma cobrança de royalty pelo identificador.</summary>
    Task<RoyaltyResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera a cobrança de royalty de uma unidade em um período já encerrado, calculada sobre
    /// o faturamento confirmado no período e o percentual vigente na unidade.
    /// </summary>
    Task<RoyaltyResponse> GerarAsync(
        GerarRoyaltyRequest requisicao,
        CancellationToken cancellationToken = default);
}
