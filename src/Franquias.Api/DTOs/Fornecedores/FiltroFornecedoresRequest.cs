namespace Franquias.Api.DTOs.Fornecedores;

/// <summary>
/// Filtros estruturados da listagem de fornecedores. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroFornecedoresRequest
{
    /// <summary>
    /// Restringe a fornecedores ativos (<c>true</c>) ou inativos (<c>false</c>). Omitido,
    /// traz os dois.
    /// </summary>
    public bool? Ativo { get; set; }

    /// <summary>
    /// Restringe aos fornecedores homologados para um item do catálogo — responde a
    /// "quem pode fornecer este produto?".
    /// </summary>
    public int? ProdutoServicoId { get; set; }
}
