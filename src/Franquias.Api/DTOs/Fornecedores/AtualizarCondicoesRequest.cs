using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;

namespace Franquias.Api.DTOs.Fornecedores;

/// <summary>
/// Novas condições comerciais negociadas com o fornecedor para um item.
/// </summary>
public class AtualizarCondicoesRequest
{
    /// <summary>Preço cobrado pelo fornecedor por unidade do item, maior que zero.</summary>
    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "O preço de fornecimento deve ser maior que zero.")]
    [CasasDecimais(2)]
    public decimal PrecoFornecimento { get; set; }

    /// <summary>Prazo de entrega acordado, em dias corridos, entre 0 e 365.</summary>
    [Range(0, 365, ErrorMessage = "O prazo de entrega deve estar entre {1} e {2} dias.")]
    public int PrazoEntregaEmDias { get; set; }
}
