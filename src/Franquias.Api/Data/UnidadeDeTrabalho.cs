namespace Franquias.Api.Data;

/// <summary>
/// Implementação de <see cref="IUnidadeDeTrabalho"/> sobre a transação do
/// <see cref="AppDbContext"/>.
/// </summary>
public sealed class UnidadeDeTrabalho(AppDbContext contexto) : IUnidadeDeTrabalho
{
    /// <inheritdoc />
    public async Task ExecutarEmTransacaoAsync(
        Func<CancellationToken, Task> operacao,
        CancellationToken cancellationToken = default)
    {
        // Se quem chamou já abriu uma transação, a operação participa dela. Abrir outra por
        // cima faria o commit interno valer antes de o chamador decidir pelo todo.
        if (contexto.Database.CurrentTransaction is not null)
        {
            await operacao(cancellationToken);
            return;
        }

        await using var transacao = await contexto.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await operacao(cancellationToken);
            await transacao.CommitAsync(cancellationToken);
        }
        catch
        {
            // O rollback não usa o token da requisição: uma requisição cancelada no meio da
            // operação é justamente o caso em que desfazer não pode ser pulado.
            await transacao.RollbackAsync(CancellationToken.None);

            // Desfazer no banco não basta. As entidades alteradas continuam rastreadas como
            // modificadas, e a próxima gravação no mesmo contexto as enviaria de novo,
            // reaplicando por outro caminho exatamente o que acabou de ser desfeito.
            contexto.ChangeTracker.Clear();

            throw;
        }
    }
}
