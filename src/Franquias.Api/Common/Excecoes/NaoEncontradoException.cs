namespace Franquias.Api.Common.Excecoes;

/// <summary>
/// Lançada quando o recurso solicitado não existe na base.
/// </summary>
public sealed class NaoEncontradoException : DominioException
{
    public NaoEncontradoException(string mensagem)
        : base(mensagem)
    {
    }

    /// <summary>
    /// Monta a mensagem padrão a partir do nome do recurso e do identificador procurado.
    /// </summary>
    public NaoEncontradoException(string recurso, object identificador)
        : base($"{recurso} com identificador {identificador} não foi encontrado.")
    {
    }
}
