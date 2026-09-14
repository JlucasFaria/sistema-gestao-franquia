using System.ComponentModel.DataAnnotations;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Vendas;

/// <summary>
/// Filtros da consulta de vendas. Todos são opcionais e cumulativos.
/// </summary>
public class FiltroVendasRequest : IValidatableObject
{
    /// <summary>Restringe às vendas de uma unidade.</summary>
    public int? UnidadeFranqueadaId { get; set; }

    /// <summary>
    /// Primeiro dia do intervalo, inclusive. As datas são comparadas em UTC, o mesmo fuso em
    /// que a data da venda é gravada.
    /// </summary>
    public DateOnly? DataInicial { get; set; }

    /// <summary>Último dia do intervalo, inclusive.</summary>
    public DateOnly? DataFinal { get; set; }

    /// <summary>Restringe a um estágio da venda.</summary>
    public StatusVenda? Status { get; set; }

    /// <summary>
    /// Recusa intervalo invertido. Sem essa checagem, a consulta simplesmente voltaria vazia
    /// e o cliente não saberia que errou as datas.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataInicial is not null && DataFinal is not null && DataInicial > DataFinal)
        {
            yield return new ValidationResult(
                "A data inicial não pode ser posterior à data final.",
                [nameof(DataInicial), nameof(DataFinal)]);
        }
    }
}
