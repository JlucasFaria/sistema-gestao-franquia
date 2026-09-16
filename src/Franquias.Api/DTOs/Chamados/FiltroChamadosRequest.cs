using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Chamados;

/// <summary>
/// Filtros da consulta de chamados. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroChamadosRequest
{
    /// <summary>Restringe aos chamados de uma unidade.</summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>Restringe a um estágio de atendimento.</summary>
    public StatusChamado? Status { get; set; }

    /// <summary>Restringe a um grau de urgência.</summary>
    public PrioridadeChamado? Prioridade { get; set; }

    /// <summary>Restringe a uma categoria, comparada sem diferenciar maiúsculas de minúsculas.</summary>
    public string? Categoria { get; set; }

    /// <summary>
    /// Quando verdadeiro, traz apenas os chamados que ainda não foram encerrados.
    /// </summary>
    public bool? SomenteEmAberto { get; set; }
}
