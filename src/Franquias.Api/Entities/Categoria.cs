namespace Franquias.Api.Entities;

/// <summary>
/// Agrupamento usado para classificar os produtos e serviços do catálogo da rede.
/// </summary>
public class Categoria : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected Categoria()
    {
    }

    public Categoria(string nome, string? descricao = null)
    {
        Nome = nome;
        Descricao = descricao;
    }

    /// <summary>Nome da categoria. Único em toda a base.</summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>Descrição do que a categoria agrupa.</summary>
    public string? Descricao { get; private set; }

    /// <summary>
    /// Altera nome e descrição da categoria, registrando a data da alteração.
    /// </summary>
    public void Atualizar(string nome, string? descricao)
    {
        Nome = nome;
        Descricao = descricao;
        RegistrarAtualizacao();
    }
}
