using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Vendas;

/// <summary>
/// Dados para registro de uma venda. O valor total não é informado: é calculado a partir
/// dos itens.
/// </summary>
public class CriarVendaRequest
{
    /// <summary>Unidade que realizou a venda.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma unidade válida.")]
    public int UnidadeFranqueadaId { get; set; }

    /// <summary>Itens vendidos. A venda precisa de pelo menos um.</summary>
    [Required(ErrorMessage = "A venda precisa de pelo menos um item.")]
    [MinLength(1, ErrorMessage = "A venda precisa de pelo menos um item.")]
    public List<ItemVendaRequest> Itens { get; set; } = [];
}
