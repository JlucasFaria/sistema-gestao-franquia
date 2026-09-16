using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Quantidade de chamados em um estágio de atendimento.
/// </summary>
/// <param name="Status">Estágio: Aberto, EmAtendimento, Resolvido ou Encerrado.</param>
/// <param name="Quantidade">Chamados nesse estágio.</param>
public sealed record ContagemPorStatusResponse(StatusChamado Status, int Quantidade);
