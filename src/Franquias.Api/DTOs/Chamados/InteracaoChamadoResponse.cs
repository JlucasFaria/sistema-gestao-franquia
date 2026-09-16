using Franquias.Api.Entities;

namespace Franquias.Api.DTOs.Chamados;

/// <summary>
/// Mensagem da linha do tempo de um chamado.
/// </summary>
/// <param name="Id">Identificador da interação.</param>
/// <param name="UsuarioId">Autor da mensagem.</param>
/// <param name="Usuario">Nome do autor da mensagem.</param>
/// <param name="Mensagem">Conteúdo registrado.</param>
/// <param name="DataRegistro">Momento do registro, em UTC.</param>
public sealed record InteracaoChamadoResponse(
    int Id,
    int UsuarioId,
    string Usuario,
    string Mensagem,
    DateTime DataRegistro)
{
    /// <summary>
    /// Projeta a entidade no DTO de saída. Espera o usuário carregado.
    /// </summary>
    public static InteracaoChamadoResponse De(InteracaoChamado interacao) => new(
        interacao.Id,
        interacao.UsuarioId,
        interacao.Usuario?.Nome ?? string.Empty,
        interacao.Mensagem,
        interacao.DataCriacao);
}
