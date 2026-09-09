namespace Franquias.Api.Common.Excecoes;

/// <summary>
/// Lançada quando a requisição está bem formada, mas contraria uma regra de negócio —
/// vender por uma unidade inativa, dar baixa sem saldo em estoque, cancelar uma venda
/// já cancelada.
/// </summary>
public sealed class RegraDeNegocioException : DominioException
{
    public RegraDeNegocioException(string mensagem)
        : base(mensagem)
    {
    }
}
