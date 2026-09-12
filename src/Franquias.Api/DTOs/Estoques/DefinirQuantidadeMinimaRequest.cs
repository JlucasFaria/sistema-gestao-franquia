using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Estoques;

/// <summary>
/// Ponto de reposição de um item na unidade.
/// </summary>
public class DefinirQuantidadeMinimaRequest
{
    /// <summary>Saldo abaixo do qual o item passa a constar como pendente de reposição.</summary>
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade mínima não pode ser negativa.")]
    public int QuantidadeMinima { get; set; }
}
