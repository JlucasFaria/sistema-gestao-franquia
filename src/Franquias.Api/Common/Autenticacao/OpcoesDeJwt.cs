namespace Franquias.Api.Common.Autenticacao;

/// <summary>
/// Parâmetros de emissão e validação do token JWT, lidos da seção <c>Jwt</c> da configuração.
/// </summary>
public class OpcoesDeJwt
{
    /// <summary>Nome da seção correspondente no arquivo de configuração.</summary>
    public const string Secao = "Jwt";

    /// <summary>Quem emite o token.</summary>
    public string Emissor { get; set; } = string.Empty;

    /// <summary>Para quem o token se destina.</summary>
    public string Audiencia { get; set; } = string.Empty;

    /// <summary>Chave de assinatura HMAC-SHA256.</summary>
    public string ChaveSecreta { get; set; } = string.Empty;

    /// <summary>Tempo de validade do token, em minutos.</summary>
    public int ExpiracaoEmMinutos { get; set; }
}
