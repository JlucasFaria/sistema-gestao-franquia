namespace Franquias.Api.Entities;

/// <summary>
/// Mensagem registrada na linha do tempo de um chamado de suporte. As interações são
/// imutáveis: compõem o histórico do atendimento.
/// </summary>
public class InteracaoChamado : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected InteracaoChamado()
    {
    }

    internal InteracaoChamado(int usuarioId, string mensagem)
    {
        UsuarioId = usuarioId;
        Mensagem = mensagem;
    }

    /// <summary>Chave estrangeira do chamado a que a interação pertence.</summary>
    public int ChamadoSuporteId { get; private set; }

    /// <summary>Chamado a que a interação pertence.</summary>
    public ChamadoSuporte ChamadoSuporte { get; private set; } = null!;

    /// <summary>Chave estrangeira do usuário autor da mensagem.</summary>
    public int UsuarioId { get; private set; }

    /// <summary>Usuário autor da mensagem.</summary>
    public Usuario Usuario { get; private set; } = null!;

    /// <summary>Conteúdo da mensagem.</summary>
    public string Mensagem { get; private set; } = string.Empty;
}
