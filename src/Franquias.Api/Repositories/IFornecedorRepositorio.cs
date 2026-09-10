using Franquias.Api.Common.Consultas;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados de fornecedores.
/// </summary>
public interface IFornecedorRepositorio : IRepositorio<Fornecedor>
{
    /// <summary>
    /// Indica se o CNPJ já pertence a algum fornecedor. O valor é normalizado antes da
    /// comparação, de modo que máscaras diferentes não escapem da checagem.
    /// </summary>
    /// <param name="cnpj">CNPJ a verificar, com ou sem máscara.</param>
    /// <param name="idIgnorado">Fornecedor a desconsiderar na checagem.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<bool> ExisteComCnpjAsync(
        string cnpj,
        int? idIgnorado = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um fornecedor com os vínculos e os respectivos itens do catálogo carregados,
    /// rastreado para que a homologação de produtos passe pelo agregado.
    /// </summary>
    Task<Fornecedor?> ObterComProdutosAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Lista os fornecedores de forma paginada.</summary>
    Task<PagedResult<Fornecedor>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default);
}
