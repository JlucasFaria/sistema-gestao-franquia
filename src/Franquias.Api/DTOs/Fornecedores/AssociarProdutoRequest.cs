using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;

namespace Franquias.Api.DTOs.Fornecedores;

/// <summary>
/// Homologação de um fornecedor para um item do catálogo.
/// </summary>
public class AssociarProdutoRequest
{
    /// <summary>Item do catálogo — produto ou serviço — que o fornecedor passa a fornecer.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe um produto ou serviço válido.")]
    public int ProdutoServicoId { get; set; }

    /// <summary>Preço cobrado pelo fornecedor por unidade do item, maior que zero.</summary>
    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "O preço de fornecimento deve ser maior que zero.")]
    [CasasDecimais(2)]
    public decimal PrecoFornecimento { get; set; }

    /// <summary>Prazo de entrega acordado, em dias corridos, entre 0 e 365.</summary>
    [Range(0, 365, ErrorMessage = "O prazo de entrega deve estar entre {1} e {2} dias.")]
    public int PrazoEntregaEmDias { get; set; }
}
