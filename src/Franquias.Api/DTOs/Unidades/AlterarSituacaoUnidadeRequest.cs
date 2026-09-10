using System.ComponentModel.DataAnnotations;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Unidades;

/// <summary>
/// Nova situação contratual de uma unidade.
/// </summary>
public class AlterarSituacaoUnidadeRequest
{
    /// <summary>Situação a aplicar: EmImplantacao, Ativa, Suspensa ou Encerrada.</summary>
    [Required(ErrorMessage = "A situação é obrigatória.")]
    [EnumDataType(typeof(SituacaoUnidade), ErrorMessage = "Situação inválida.")]
    public SituacaoUnidade? Situacao { get; set; }
}
