namespace Franquias.Api.Common.Autenticacao;

/// <summary>
/// Nomes das políticas de autorização. As políticas são cumulativas: quem tem mais
/// permissão atende também às exigências das camadas abaixo, de modo que um administrador
/// nunca é barrado em um endpoint operacional.
/// </summary>
public static class PoliticasDeAcesso
{
    /// <summary>Exclusiva do administrador da rede.</summary>
    public const string Administrador = nameof(Administrador);

    /// <summary>Administrador ou gestor de unidade.</summary>
    public const string GestorDeUnidade = nameof(GestorDeUnidade);

    /// <summary>Qualquer perfil operacional: administrador, gestor de unidade ou operador.</summary>
    public const string Operador = nameof(Operador);
}
