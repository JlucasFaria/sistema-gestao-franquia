using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Chamado aberto por uma unidade franqueada junto ao suporte da franqueadora.
/// </summary>
public class ChamadoSuporte : EntidadeBase
{
    private readonly List<InteracaoChamado> _interacoes = [];

    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected ChamadoSuporte()
    {
    }

    public ChamadoSuporte(
        int unidadeFranqueadaId,
        int usuarioAberturaId,
        string categoria,
        string assunto,
        string descricao,
        PrioridadeChamado prioridade)
    {
        UnidadeFranqueadaId = unidadeFranqueadaId;
        UsuarioAberturaId = usuarioAberturaId;
        Categoria = categoria;
        Assunto = assunto;
        Descricao = descricao;
        Prioridade = prioridade;
        Status = StatusChamado.Aberto;
    }

    /// <summary>Chave estrangeira da unidade que abriu o chamado.</summary>
    public int UnidadeFranqueadaId { get; private set; }

    /// <summary>Unidade que abriu o chamado.</summary>
    public UnidadeFranqueada UnidadeFranqueada { get; private set; } = null!;

    /// <summary>Chave estrangeira do usuário que abriu o chamado.</summary>
    public int UsuarioAberturaId { get; private set; }

    /// <summary>Usuário que abriu o chamado.</summary>
    public Usuario UsuarioAbertura { get; private set; } = null!;

    /// <summary>Assunto geral do chamado, como sistema, logística ou marketing.</summary>
    public string Categoria { get; private set; } = string.Empty;

    /// <summary>Resumo do problema relatado.</summary>
    public string Assunto { get; private set; } = string.Empty;

    /// <summary>Descrição detalhada do problema relatado.</summary>
    public string Descricao { get; private set; } = string.Empty;

    /// <summary>Grau de urgência do chamado.</summary>
    public PrioridadeChamado Prioridade { get; private set; }

    /// <summary>Estágio do atendimento.</summary>
    public StatusChamado Status { get; private set; }

    /// <summary>Momento do encerramento, em UTC. Nulo enquanto o chamado não foi encerrado.</summary>
    public DateTime? DataEncerramento { get; private set; }

    /// <summary>Linha do tempo do atendimento.</summary>
    public IReadOnlyCollection<InteracaoChamado> Interacoes => _interacoes;

    /// <summary>
    /// Indica se o chamado ainda demanda atenção do suporte, ou seja, se não foi encerrado.
    /// </summary>
    public bool EstaEmAberto() => Status != StatusChamado.Encerrado;

    /// <summary>
    /// Acrescenta uma mensagem à linha do tempo do chamado.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se o chamado já estiver encerrado.</exception>
    public InteracaoChamado RegistrarInteracao(int usuarioId, string mensagem)
    {
        ExigirChamadoNaoEncerrado();

        var interacao = new InteracaoChamado(usuarioId, mensagem);

        _interacoes.Add(interacao);
        RegistrarAtualizacao();

        return interacao;
    }

    /// <summary>
    /// Reclassifica a urgência do chamado.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se o chamado já estiver encerrado.</exception>
    public void AlterarPrioridade(PrioridadeChamado prioridade)
    {
        ExigirChamadoNaoEncerrado();

        Prioridade = prioridade;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Avança o chamado para outro estágio de atendimento. Ao encerrar, carimba a data de
    /// encerramento.
    /// </summary>
    /// <exception cref="InvalidOperationException">Se o chamado já estiver encerrado.</exception>
    public void AlterarStatus(StatusChamado status)
    {
        ExigirChamadoNaoEncerrado();

        Status = status;

        if (status == StatusChamado.Encerrado)
        {
            DataEncerramento = DateTime.UtcNow;
        }

        RegistrarAtualizacao();
    }

    private void ExigirChamadoNaoEncerrado()
    {
        if (Status == StatusChamado.Encerrado)
        {
            throw new InvalidOperationException("O chamado está encerrado e não aceita novas alterações.");
        }
    }
}
