namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Grau de urgência atribuído a um chamado de suporte.
/// </summary>
public enum PrioridadeChamado
{
    /// <summary>Sem impacto na operação da unidade.</summary>
    Baixa = 1,

    /// <summary>Impacto moderado, com solução de contorno disponível.</summary>
    Media = 2,

    /// <summary>Impacto relevante na operação da unidade.</summary>
    Alta = 3,

    /// <summary>Operação da unidade paralisada.</summary>
    Critica = 4
}
