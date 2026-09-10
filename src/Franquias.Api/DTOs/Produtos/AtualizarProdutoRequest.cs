using System.ComponentModel.DataAnnotations;
using Franquias.Api.Common.Validacoes;

namespace Franquias.Api.DTOs.Produtos;

/// <summary>
/// Dados de catálogo alteráveis de um item. A natureza — produto ou serviço — não consta:
/// trocá-la depois de haver movimentação de estoque deixaria saldos sem sentido.
/// </summary>
public class AtualizarProdutoRequest
{
    /// <summary>Categoria do item.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma categoria válida.")]
    public int CategoriaId { get; set; }

    /// <summary>Nome comercial do item.</summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>Descrição detalhada do item.</summary>
    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
    public string? Descricao { get; set; }

    /// <summary>Preço de tabela sugerido pela franqueadora, maior que zero.</summary>
    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "O preço base deve ser maior que zero.")]
    [CasasDecimais(2)]
    public decimal PrecoBase { get; set; }
}
