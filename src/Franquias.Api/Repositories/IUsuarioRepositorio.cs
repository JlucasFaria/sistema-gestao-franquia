using Franquias.Api.Common.Consultas;
using Franquias.Api.Entities;

namespace Franquias.Api.Repositories;

/// <summary>
/// Acesso a dados de usuários, acrescentando ao repositório genérico as consultas
/// específicas da entidade.
/// </summary>
public interface IUsuarioRepositorio : IRepositorio<Usuario>
{
    /// <summary>
    /// Busca um usuário pelo e-mail, já com o perfil carregado. O valor informado é
    /// normalizado antes da comparação.
    /// </summary>
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indica se o e-mail já pertence a algum usuário.
    /// </summary>
    /// <param name="email">E-mail a verificar.</param>
    /// <param name="idIgnorado">
    /// Usuário a desconsiderar na checagem. Ao editar um cadastro, o próprio registro não
    /// pode ser tratado como duplicata dele mesmo.
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<bool> ExisteComEmailAsync(
        string email,
        int? idIgnorado = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista os usuários de forma paginada, com o perfil carregado, aplicando busca por
    /// nome ou e-mail e filtro por situação.
    /// </summary>
    Task<PagedResult<Usuario>> ListarAsync(
        QueryParams parametros,
        bool? apenasAtivos,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta quantos administradores ativos existem. Sustenta a regra que impede a rede de
    /// ficar sem ninguém capaz de administrá-la.
    /// </summary>
    Task<int> ContarAdministradoresAtivosAsync(CancellationToken cancellationToken = default);
}
