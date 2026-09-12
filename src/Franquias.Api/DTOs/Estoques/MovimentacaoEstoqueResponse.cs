using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Estoques;

/// <summary>
/// Registro histórico de uma movimentação de estoque.
/// </summary>
/// <param name="Id">Identificador da movimentação.</param>
/// <param name="EstoqueId">Saldo de estoque movimentado.</param>
/// <param name="Tipo">Natureza da movimentação: Entrada, Saida ou Ajuste.</param>
/// <param name="Quantidade">Quantidade movimentada, sempre positiva.</param>
/// <param name="SaldoAnterior">Saldo antes da movimentação.</param>
/// <param name="SaldoResultante">Saldo depois da movimentação.</param>
/// <param name="Observacao">Justificativa registrada.</param>
/// <param name="DataMovimentacao">Momento da movimentação, em UTC.</param>
public sealed record MovimentacaoEstoqueResponse(
    int Id,
    int EstoqueId,
    TipoMovimentacao Tipo,
    int Quantidade,
    int SaldoAnterior,
    int SaldoResultante,
    string? Observacao,
    DateTime DataMovimentacao)
{
    /// <summary>
    /// Projeta a entidade no DTO de saída. A movimentação é imutável, então o instante em
    /// que ela foi registrada é a própria data de criação do registro.
    /// </summary>
    public static MovimentacaoEstoqueResponse De(MovimentacaoEstoque movimentacao) => new(
        movimentacao.Id,
        movimentacao.EstoqueId,
        movimentacao.Tipo,
        movimentacao.Quantidade,
        movimentacao.SaldoAnterior,
        movimentacao.SaldoResultante,
        movimentacao.Observacao,
        movimentacao.DataCriacao);
}
