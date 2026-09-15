using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.DTOs.Chamados;

/// <summary>
/// Chamado de suporte devolvido pela API, com a linha do tempo do atendimento.
/// </summary>
/// <param name="Id">Identificador do chamado.</param>
/// <param name="UnidadeFranqueadaId">Unidade que abriu o chamado.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="UsuarioAberturaId">Usuário que abriu o chamado.</param>
/// <param name="UsuarioAbertura">Nome de quem abriu o chamado.</param>
/// <param name="Categoria">Assunto geral, como sistema, logística ou marketing.</param>
/// <param name="Assunto">Resumo do problema relatado.</param>
/// <param name="Descricao">Descrição detalhada do problema.</param>
/// <param name="Prioridade">Grau de urgência: Baixa, Media, Alta ou Critica.</param>
/// <param name="Status">Estágio: Aberto, EmAtendimento, Resolvido ou Encerrado.</param>
/// <param name="EmAberto">Indica se o chamado ainda demanda atenção do suporte.</param>
/// <param name="DataAbertura">Momento da abertura, em UTC.</param>
/// <param name="DataEncerramento">Momento do encerramento, em UTC, se já encerrado.</param>
/// <param name="DataAtualizacao">Momento da última alteração, em UTC.</param>
/// <param name="Interacoes">Linha do tempo do atendimento, da mais antiga para a mais recente.</param>
public sealed record ChamadoResponse(
    int Id,
    int UnidadeFranqueadaId,
    string Unidade,
    int UsuarioAberturaId,
    string UsuarioAbertura,
    string Categoria,
    string Assunto,
    string Descricao,
    PrioridadeChamado Prioridade,
    StatusChamado Status,
    bool EmAberto,
    DateTime DataAbertura,
    DateTime? DataEncerramento,
    DateTime? DataAtualizacao,
    IReadOnlyCollection<InteracaoChamadoResponse> Interacoes)
{
    /// <summary>
    /// Projeta a entidade no DTO de saída. Espera a unidade, o autor da abertura e as
    /// interações, com seus autores, carregados.
    /// </summary>
    public static ChamadoResponse De(ChamadoSuporte chamado) => new(
        chamado.Id,
        chamado.UnidadeFranqueadaId,
        chamado.UnidadeFranqueada?.NomeFantasia ?? string.Empty,
        chamado.UsuarioAberturaId,
        chamado.UsuarioAbertura?.Nome ?? string.Empty,
        chamado.Categoria,
        chamado.Assunto,
        chamado.Descricao,
        chamado.Prioridade,
        chamado.Status,
        chamado.EstaEmAberto(),
        chamado.DataCriacao,
        chamado.DataEncerramento,
        chamado.DataAtualizacao,
        [.. chamado.Interacoes.OrderBy(interacao => interacao.Id).Select(InteracaoChamadoResponse.De)]);
}
