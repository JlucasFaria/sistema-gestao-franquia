using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.Common.Validacoes;

/// <summary>
/// Valida que a propriedade contém um CNPJ com dígitos verificadores corretos.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class CnpjAttribute : ValidationAttribute
{
    public CnpjAttribute()
        : base("O CNPJ informado não é válido.")
    {
    }

    /// <summary>
    /// Valor nulo é considerado válido: a obrigatoriedade do campo é responsabilidade do
    /// <see cref="RequiredAttribute"/>, não deste atributo.
    /// </summary>
    public override bool IsValid(object? value) =>
        value is null || (value is string texto && ValidadorDeDocumento.EhCnpjValido(texto));
}
