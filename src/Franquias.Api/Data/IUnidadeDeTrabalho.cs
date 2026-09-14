namespace Franquias.Api.Data;

/// <summary>
/// Execução de operações que precisam gravar em várias tabelas de forma atômica: ou tudo é
/// confirmado, ou nada é.
/// </summary>
public interface IUnidadeDeTrabalho
{
    /// <summary>
    /// Executa a operação dentro de uma transação. Se a operação lançar qualquer exceção, a
    /// transação é desfeita e as alterações pendentes do contexto são descartadas.
    /// </summary>
    /// <param name="operacao">Trabalho a executar. Recebe o token de cancelamento.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task ExecutarEmTransacaoAsync(
        Func<CancellationToken, Task> operacao,
        CancellationToken cancellationToken = default);
}
