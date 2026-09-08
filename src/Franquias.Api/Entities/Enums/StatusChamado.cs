namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Estágio de um chamado de suporte aberto por uma unidade.
/// </summary>
public enum StatusChamado
{
    /// <summary>Chamado registrado e aguardando triagem.</summary>
    Aberto = 1,

    /// <summary>Chamado sendo tratado pela equipe de suporte.</summary>
    EmAtendimento = 2,

    /// <summary>Solução aplicada, aguardando confirmação da unidade.</summary>
    Resolvido = 3,

    /// <summary>Chamado finalizado; não recebe novas interações.</summary>
    Encerrado = 4
}
