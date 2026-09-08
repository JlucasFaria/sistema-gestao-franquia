namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Situação contratual de uma unidade franqueada.
/// </summary>
public enum SituacaoUnidade
{
    /// <summary>Contrato assinado, mas a unidade ainda não iniciou a operação.</summary>
    EmImplantacao = 1,

    /// <summary>Unidade em operação regular.</summary>
    Ativa = 2,

    /// <summary>Operação temporariamente interrompida.</summary>
    Suspensa = 3,

    /// <summary>Contrato encerrado; a unidade não opera mais.</summary>
    Encerrada = 4
}
