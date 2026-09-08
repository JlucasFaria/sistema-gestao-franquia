using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Item do catálogo da rede, que pode ser um produto físico ou um serviço prestado
/// pelas unidades.
/// </summary>
public class ProdutoServico : EntidadeBase
{
    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected ProdutoServico()
    {
    }

    public ProdutoServico(
        int categoriaId,
        string nome,
        string? descricao,
        decimal precoBase,
        bool ehServico)
    {
        CategoriaId = categoriaId;
        Nome = nome;
        Descricao = descricao;
        PrecoBase = precoBase;
        EhServico = ehServico;
        Status = StatusProduto.Ativo;
    }

    /// <summary>Chave estrangeira da categoria do item.</summary>
    public int CategoriaId { get; private set; }

    /// <summary>Categoria do item.</summary>
    public Categoria Categoria { get; private set; } = null!;

    /// <summary>Nome comercial do item.</summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>Descrição detalhada do item.</summary>
    public string? Descricao { get; private set; }

    /// <summary>Preço de tabela sugerido pela franqueadora.</summary>
    public decimal PrecoBase { get; private set; }

    /// <summary>Situação do item no catálogo.</summary>
    public StatusProduto Status { get; private set; }

    /// <summary>
    /// Indica que o item é um serviço. Serviços não são controlados em estoque, por não
    /// terem existência física.
    /// </summary>
    public bool EhServico { get; private set; }

    /// <summary>
    /// Indica se o item pode ser vendido pelas unidades.
    /// </summary>
    public bool EstaDisponivelParaVenda() => Ativo && Status == StatusProduto.Ativo;

    /// <summary>
    /// Indica se o item movimenta estoque.
    /// </summary>
    public bool ControlaEstoque() => !EhServico;

    /// <summary>
    /// Atualiza os dados de catálogo do item, registrando a data da alteração.
    /// </summary>
    public void Atualizar(int categoriaId, string nome, string? descricao, decimal precoBase)
    {
        CategoriaId = categoriaId;
        Nome = nome;
        Descricao = descricao;
        PrecoBase = precoBase;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Altera a situação do item no catálogo.
    /// </summary>
    public void AlterarStatus(StatusProduto status)
    {
        Status = status;
        RegistrarAtualizacao();
    }
}
