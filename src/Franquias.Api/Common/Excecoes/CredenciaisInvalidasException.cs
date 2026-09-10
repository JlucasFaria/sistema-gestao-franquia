namespace Franquias.Api.Common.Excecoes;

/// <summary>
/// Lançada quando a autenticação é recusada, seja por credencial incorreta, seja por conta
/// inativa. A mensagem é deliberadamente genérica nos casos de credencial: informar que o
/// e-mail existe, mas a senha está errada, entregaria a um atacante a confirmação de quais
/// contas existem na base.
/// </summary>
public sealed class CredenciaisInvalidasException : DominioException
{
    public CredenciaisInvalidasException(string mensagem)
        : base(mensagem)
    {
    }
}
