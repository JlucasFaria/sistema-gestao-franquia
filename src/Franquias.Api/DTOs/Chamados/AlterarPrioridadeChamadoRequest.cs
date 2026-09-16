using System.ComponentModel.DataAnnotations;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Chamados;

/// <summary>
/// Nova prioridade atribuída a um chamado em andamento.
/// </summary>
public class AlterarPrioridadeChamadoRequest
{
    /// <summary>Grau de urgência: Baixa, Media, Alta ou Critica.</summary>
    [Required(ErrorMessage = "Informe a prioridade do chamado.")]
    [EnumDataType(typeof(PrioridadeChamado), ErrorMessage = "Prioridade inválida.")]
    public PrioridadeChamado? Prioridade { get; set; }
}
