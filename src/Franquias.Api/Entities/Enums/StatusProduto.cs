namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Situação de um produto ou serviço no catálogo da rede.
/// </summary>
public enum StatusProduto
{
    /// <summary>Disponível para venda e reposição.</summary>
    Ativo = 1,

    /// <summary>Temporariamente indisponível para venda.</summary>
    Inativo = 2,

    /// <summary>Retirado do catálogo em definitivo.</summary>
    Descontinuado = 3
}
