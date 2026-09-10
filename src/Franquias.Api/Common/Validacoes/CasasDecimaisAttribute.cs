using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Franquias.Api.Common.Validacoes;

/// <summary>
/// Limita a quantidade de casas decimais de um valor <see cref="decimal"/>. O SQLite grava
/// decimais como texto e não impõe a precisão declarada no mapeamento, então um preço como
/// 10,999 seria persistido exatamente assim se nada o barrasse na entrada.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class CasasDecimaisAttribute : ValidationAttribute
{
    public CasasDecimaisAttribute(int maximo)
        : base("O campo {0} deve ter no máximo {1} casas decimais.")
    {
        Maximo = maximo;
    }

    /// <summary>Quantidade máxima de casas decimais aceitas.</summary>
    public int Maximo { get; }

    /// <summary>
    /// Valor nulo é considerado válido: a obrigatoriedade é responsabilidade do
    /// <see cref="RequiredAttribute"/>.
    /// </summary>
    public override bool IsValid(object? value) => value switch
    {
        null => true,
        decimal valor => decimal.Round(valor, Maximo) == valor,
        _ => false
    };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Maximo);
}
