namespace Franquias.Api.Common.Consultas;

/// <summary>
/// Página de resultados devolvida pelos endpoints de listagem, acompanhada dos metadados
/// necessários para o cliente navegar sem adivinhar quantas páginas existem.
/// </summary>
/// <typeparam name="T">Tipo dos itens retornados, normalmente um DTO de saída.</typeparam>
public class PagedResult<T>
{
    public PagedResult(IReadOnlyCollection<T> itens, int total, int pagina, int tamanhoPagina)
    {
        Itens = itens;
        Total = total;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
    }

    /// <summary>Itens da página atual.</summary>
    public IReadOnlyCollection<T> Itens { get; }

    /// <summary>Total de registros que atendem ao filtro, ignorando a paginação.</summary>
    public int Total { get; }

    /// <summary>Página atual.</summary>
    public int Pagina { get; }

    /// <summary>Itens por página.</summary>
    public int TamanhoPagina { get; }

    /// <summary>Quantidade de páginas disponíveis.</summary>
    public int TotalDePaginas => TamanhoPagina <= 0
        ? 0
        : (int)Math.Ceiling(Total / (double)TamanhoPagina);

    /// <summary>Indica se existe página anterior.</summary>
    public bool TemPaginaAnterior => Pagina > 1;

    /// <summary>Indica se existe página seguinte.</summary>
    public bool TemProximaPagina => Pagina < TotalDePaginas;

    /// <summary>Página vazia, usada quando o filtro não retorna nenhum registro.</summary>
    public static PagedResult<T> Vazia(QueryParams parametros) =>
        new([], 0, parametros.Pagina, parametros.TamanhoPagina);
}
