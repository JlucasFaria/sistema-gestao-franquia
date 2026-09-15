using System.ComponentModel.DataAnnotations;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Chamados;

/// <summary>
/// Novo estágio de atendimento de um chamado.
/// </summary>
public class AlterarStatusChamadoRequest
{
    /// <summary>Estágio: Aberto, EmAtendimento, Resolvido ou Encerrado.</summary>
    [Required(ErrorMessage = "Informe o status do chamado.")]
    [EnumDataType(typeof(StatusChamado), ErrorMessage = "Status inválido.")]
    public StatusChamado? Status { get; set; }

    /// <summary>
    /// Observação registrada na linha do tempo junto com a mudança de estágio. Opcional.
    /// </summary>
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "A observação deve ter de 2 a 2000 caracteres.")]
    public string? Observacao { get; set; }
}
