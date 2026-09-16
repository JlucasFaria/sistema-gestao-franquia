namespace Franquias.Api.DTOs.Relatorios;

/// <summary>
/// Item de estoque que caiu abaixo da quantidade mínima definida para a unidade.
/// </summary>
/// <param name="UnidadeFranqueadaId">Identificador da unidade.</param>
/// <param name="Unidade">Nome fantasia da unidade.</param>
/// <param name="ProdutoServicoId">Identificador do produto.</param>
/// <param name="Produto">Nome do produto.</param>
/// <param name="Categoria">Categoria do produto.</param>
/// <param name="Quantidade">Saldo atual em estoque.</param>
/// <param name="QuantidadeMinima">Saldo mínimo definido para o item na unidade.</param>
/// <param name="Falta">Quanto falta para voltar ao mínimo.</param>
public sealed record EstoqueCriticoResponse(
    int UnidadeFranqueadaId,
    string Unidade,
    int ProdutoServicoId,
    string Produto,
    string Categoria,
    int Quantidade,
    int QuantidadeMinima,
    int Falta);
