namespace Franquias.Api.Entities.Enums;

/// <summary>
/// Perfis de acesso que definem o que cada usuário pode fazer no sistema.
/// </summary>
public enum PerfilAcesso
{
    /// <summary>Acesso total à rede, incluindo cadastros da franqueadora e relatórios consolidados.</summary>
    Administrador = 1,

    /// <summary>Responsável por uma unidade franqueada; gerencia estoque, vendas e chamados da própria unidade.</summary>
    GestorUnidade = 2,

    /// <summary>Acesso operacional restrito ao registro de vendas e movimentações de estoque.</summary>
    Operador = 3
}
