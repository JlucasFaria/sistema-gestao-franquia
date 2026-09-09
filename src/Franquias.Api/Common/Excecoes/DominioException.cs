namespace Franquias.Api.Common.Excecoes;

/// <summary>
/// Base das exceções previsíveis do domínio, ou seja, falhas causadas pelo que foi pedido
/// e não por defeito do sistema. Serve de ponto único para o middleware distinguir o que
/// deve virar resposta de erro tratada do que é falha inesperada.
/// </summary>
public abstract class DominioException : Exception
{
    protected DominioException(string mensagem)
        : base(mensagem)
    {
    }
}
