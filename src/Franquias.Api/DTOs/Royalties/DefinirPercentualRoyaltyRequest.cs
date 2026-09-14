using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;

namespace Franquias.Api.DTOs.Royalties;

/// <summary>
/// Novo percentual de royalty de uma unidade.
/// </summary>
public class DefinirPercentualRoyaltyRequest
{
    /// <summary>
    /// Percentual sobre o faturamento, entre 0 e 100, com até duas casas decimais. Vale para
    /// as próximas cobranças; as já emitidas mantêm o percentual com que foram geradas.
    /// </summary>
    [Range(typeof(decimal), "0", "100", ErrorMessage = "O percentual de royalty deve estar entre {1} e {2}.")]
    [CasasDecimais(2)]
    public decimal PercentualRoyalty { get; set; }
}
