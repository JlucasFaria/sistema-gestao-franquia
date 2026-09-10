namespace Franquias.Api.Common.Consultas;

/// <summary>
/// Parâmetros de consulta aceitos pelos endpoints de listagem: paginação, ordenação e
/// termo de busca.
/// </summary>
public class QueryParams
{
    /// <summary>Teto de itens por página, para que um cliente não peça a base inteira.</summary>
    public const int TamanhoMaximoDePagina = 100;

    private const int TamanhoPadraoDePagina = 20;

    private int _pagina = 1;
    private int _tamanhoPagina = TamanhoPadraoDePagina;

    /// <summary>
    /// Número da página, começando em 1. Valores menores que 1 são corrigidos para 1.
    /// </summary>
    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Quantidade de itens por página. Fora da faixa aceita, o valor é ajustado em vez de
    /// recusado: paginação é conveniência de leitura, não regra de negócio.
    /// </summary>
    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value switch
        {
            < 1 => TamanhoPadraoDePagina,
            > TamanhoMaximoDePagina => TamanhoMaximoDePagina,
            _ => value
        };
    }

    /// <summary>
    /// Termo de busca livre. Cada repositório decide em quais campos aplicá-lo, já que os
    /// campos pesquisáveis variam por entidade.
    /// </summary>
    public string? Busca { get; set; }

    /// <summary>Nome da propriedade usada para ordenar. Nulo mantém a ordem natural.</summary>
    public string? OrdenarPor { get; set; }

    /// <summary>Inverte a ordenação para decrescente.</summary>
    public bool Decrescente { get; set; }

    /// <summary>Quantidade de registros a pular para alcançar a página pedida.</summary>
    public int QuantidadeParaPular() => (Pagina - 1) * TamanhoPagina;
}
