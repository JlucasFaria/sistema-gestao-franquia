namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Resultado da marcação das cobranças vencidas como atrasadas.
/// </summary>
/// <param name="DataReferencia">Data considerada como hoje na avaliação do vencimento.</param>
/// <param name="CobrancasMarcadasComoAtrasadas">Quantidade de cobranças que passaram a atrasadas.</param>
public sealed record AtualizacaoAtrasosResponse(
    DateOnly DataReferencia,
    int CobrancasMarcadasComoAtrasadas);
