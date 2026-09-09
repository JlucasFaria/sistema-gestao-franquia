namespace Franquias.Api.Common.Excecoes;

/// <summary>
/// Lançada quando a operação esbarra em um registro já existente, como cadastrar um
/// segundo usuário com o mesmo e-mail ou uma segunda unidade com o mesmo CNPJ.
/// </summary>
public sealed class ConflitoException : DominioException
{
    public ConflitoException(string mensagem)
        : base(mensagem)
    {
    }

    /// <summary>
    /// Monta a mensagem padrão de duplicidade a partir do recurso e do campo em conflito.
    /// </summary>
    public ConflitoException(string recurso, string campo, object valor)
        : base($"Já existe {recurso} com {campo} {valor}.")
    {
    }
}
