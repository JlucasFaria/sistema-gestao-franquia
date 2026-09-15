using System.ComponentModel.DataAnnotations;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Chamados;

/// <summary>
/// Dados para a abertura de um chamado de suporte por uma unidade. O autor do chamado é
/// sempre o usuário autenticado, e por isso não é informado aqui.
/// </summary>
public class AbrirChamadoRequest
{
    /// <summary>Unidade que está abrindo o chamado.</summary>
    [Required(ErrorMessage = "Informe a unidade do chamado.")]
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma unidade válida.")]
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Assunto geral, como sistema, logística ou marketing.</summary>
    [Required(ErrorMessage = "Informe a categoria do chamado.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "A categoria deve ter de 2 a 80 caracteres.")]
    public string Categoria { get; set; } = string.Empty;

    /// <summary>Resumo do problema relatado.</summary>
    [Required(ErrorMessage = "Informe o assunto do chamado.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "O assunto deve ter de 5 a 150 caracteres.")]
    public string Assunto { get; set; } = string.Empty;

    /// <summary>Descrição detalhada do problema relatado.</summary>
    [Required(ErrorMessage = "Descreva o problema.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter de 10 a 2000 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Grau de urgência: Baixa, Media, Alta ou Critica.</summary>
    [Required(ErrorMessage = "Informe a prioridade do chamado.")]
    [EnumDataType(typeof(PrioridadeChamado), ErrorMessage = "Prioridade inválida.")]
    public PrioridadeChamado? Prioridade { get; set; }
}
