namespace Franquias.Api.DTOs.Produtos;

/// <summary>
/// Filtros estruturados da listagem do catálogo. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroProdutosRequest
{
    /// <summary>Restringe aos itens de uma categoria.</summary>
    public int? CategoriaId { get; set; }

    /// <summary>
    /// Restringe a serviços (<c>true</c>) ou a produtos físicos (<c>false</c>). Omitido,
    /// traz os dois.
    /// </summary>
    public bool? EhServico { get; set; }
}
