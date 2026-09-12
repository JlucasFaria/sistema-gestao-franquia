namespace Franquias.Api.DTOs.Estoques;

/// <summary>
/// Filtros estruturados da consulta de saldos. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroEstoquesRequest
{
    /// <summary>Restringe aos saldos de uma unidade franqueada.</summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>Restringe aos saldos de um item do catálogo, em todas as unidades.</summary>
    public int? ProdutoServicoId { get; set; }

    /// <summary>
    /// Restringe aos itens que atingiram o ponto de reposição (<c>true</c>) ou aos que ainda
    /// não atingiram (<c>false</c>). Saldo igual ao mínimo não conta como abaixo. Combinado
    /// com <see cref="UnidadeFranqueadaId"/>, responde "o que falta repor nesta loja?";
    /// sozinho, responde o mesmo para a rede inteira.
    /// </summary>
    public bool? AbaixoDoMinimo { get; set; }
}
