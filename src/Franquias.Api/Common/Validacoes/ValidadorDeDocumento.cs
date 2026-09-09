namespace Franquias.Api.Common.Validacoes;

/// <summary>
/// Validação dos dígitos verificadores de CPF e CNPJ, conforme o algoritmo de módulo 11
/// da Receita Federal.
/// </summary>
public static class ValidadorDeDocumento
{
    private static readonly int[] PesosCpfPrimeiroDigito = [10, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] PesosCpfSegundoDigito = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] PesosCnpjPrimeiroDigito = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] PesosCnpjSegundoDigito = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    /// <summary>
    /// Indica se o CPF é válido. Aceita o valor com ou sem máscara.
    /// </summary>
    public static bool EhCpfValido(string? valor) =>
        EhDocumentoValido(valor, 11, PesosCpfPrimeiroDigito, PesosCpfSegundoDigito);

    /// <summary>
    /// Indica se o CNPJ é válido. Aceita o valor com ou sem máscara.
    /// </summary>
    public static bool EhCnpjValido(string? valor) =>
        EhDocumentoValido(valor, 14, PesosCnpjPrimeiroDigito, PesosCnpjSegundoDigito);

    private static bool EhDocumentoValido(
        string? valor,
        int tamanhoEsperado,
        int[] pesosPrimeiroDigito,
        int[] pesosSegundoDigito)
    {
        var digitos = valor.SomenteDigitos();

        if (digitos.Length != tamanhoEsperado)
        {
            return false;
        }

        // Sequências repetidas (11111111111, 00000000000) satisfazem o módulo 11, mas não
        // são documentos válidos: precisam ser recusadas explicitamente.
        if (TodosOsDigitosIguais(digitos))
        {
            return false;
        }

        var quantidadeBase = pesosPrimeiroDigito.Length;

        var primeiroDigito = CalcularDigito(digitos.AsSpan(0, quantidadeBase), pesosPrimeiroDigito);
        var segundoDigito = CalcularDigito(digitos.AsSpan(0, quantidadeBase + 1), pesosSegundoDigito);

        return digitos[quantidadeBase] == primeiroDigito
            && digitos[quantidadeBase + 1] == segundoDigito;
    }

    private static char CalcularDigito(ReadOnlySpan<char> baseDoCalculo, int[] pesos)
    {
        var soma = 0;

        for (var posicao = 0; posicao < pesos.Length; posicao++)
        {
            soma += (baseDoCalculo[posicao] - '0') * pesos[posicao];
        }

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;

        return (char)(digito + '0');
    }

    private static bool TodosOsDigitosIguais(string digitos)
    {
        foreach (var digito in digitos)
        {
            if (digito != digitos[0])
            {
                return false;
            }
        }

        return true;
    }
}
