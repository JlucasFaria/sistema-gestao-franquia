using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;

namespace Franquias.Api.DTOs.Vendas;

/// <summary>
/// Item informado no registro de uma venda.
/// </summary>
public class ItemVendaRequest
{
    /// <summary>Item do catálogo vendido.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe um produto ou serviço válido.")]
    public int ProdutoServicoId { get; set; }

    /// <summary>Quantidade vendida.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade vendida deve ser maior que zero.")]
    public int Quantidade { get; set; }

    /// <summary>
    /// Preço unitário praticado pela unidade. Omitido, vale o preço de tabela do catálogo.
    /// </summary>
    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "O preço unitário deve ser maior que zero.")]
    [CasasDecimais(2)]
    public decimal? PrecoUnitario { get; set; }
}
