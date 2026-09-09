namespace Franquias.Api.Entities;

/// <summary>
/// Base comum a todas as entidades persistidas, reunindo o identificador e os campos de auditoria.
/// </summary>
public abstract class EntidadeBase
{
    /// <summary>Identificador único gerado pelo banco de dados.</summary>
    public int Id { get; protected set; }

    /// <summary>Momento em que o registro foi criado, em UTC.</summary>
    public DateTime DataCriacao { get; protected set; } = DateTime.UtcNow;

    /// <summary>Momento da última alteração, em UTC. Nulo enquanto o registro nunca foi alterado.</summary>
    public DateTime? DataAtualizacao { get; protected set; }

    /// <summary>
    /// Indica se o registro está ativo. A exclusão no sistema é lógica: registros são
    /// inativados para preservar o histórico de vendas, estoque e royalties.
    /// </summary>
    public bool Ativo { get; protected set; } = true;

    /// <summary>
    /// Reativa o registro. Não faz nada se ele já estiver ativo.
    /// </summary>
    public void Ativar()
    {
        if (Ativo)
        {
            return;
        }

        Ativo = true;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Inativa o registro, sem removê-lo do banco. Não faz nada se ele já estiver inativo.
    /// </summary>
    public void Inativar()
    {
        if (!Ativo)
        {
            return;
        }

        Ativo = false;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Carimba o momento atual como data da última alteração.
    /// </summary>
    public void RegistrarAtualizacao() => DataAtualizacao = DateTime.UtcNow;
}
