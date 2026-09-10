using Franquias.Api.Entities.Enums;

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

    /// <summary>Restringe a uma situação no catálogo: Ativo, Inativo ou Descontinuado.</summary>
    public StatusProduto? Status { get; set; }

    /// <summary>
    /// Restringe a itens com cadastro ativo (<c>true</c>) ou inativo (<c>false</c>).
    /// </summary>
    public bool? Ativo { get; set; }

    /// <summary>
    /// Restringe aos itens que podem (<c>true</c>) ou não podem (<c>false</c>) ser vendidos
    /// agora, isto é, com cadastro ativo e status Ativo ao mesmo tempo.
    /// </summary>
    public bool? DisponivelParaVenda { get; set; }
}
