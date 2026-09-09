namespace Franquias.Api.Common;

/// <summary>
/// Extensões de normalização de texto usadas pelas entidades de domínio.
/// </summary>
public static class ExtensoesDeTexto
{
    /// <summary>
    /// Remove tudo que não for dígito. Usado para gravar documentos e CEP em formato
    /// canônico, de modo que máscaras diferentes não gerem registros duplicados.
    /// </summary>
    public static string SomenteDigitos(this string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return string.Concat(valor.Where(char.IsAsciiDigit));
    }
}
