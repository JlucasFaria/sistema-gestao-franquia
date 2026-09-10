using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Usuarios;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IUsuarioService"/>.
/// </summary>
public sealed class UsuarioService(
    IUsuarioRepositorio usuarios,
    IRepositorio<Perfil> perfis,
    IHashDeSenhaService hashDeSenha) : IUsuarioService
{
    /// <inheritdoc />
    public async Task<PagedResult<UsuarioResponse>> ListarAsync(
        QueryParams parametros,
        bool? apenasAtivos,
        CancellationToken cancellationToken = default)
    {
        var pagina = await usuarios.ListarAsync(parametros, apenasAtivos, cancellationToken);

        return pagina.Converter(UsuarioResponse.De);
    }

    /// <inheritdoc />
    public async Task<UsuarioResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var usuario = await BuscarOuFalharAsync(id, cancellationToken);

        return UsuarioResponse.De(usuario);
    }

    /// <inheritdoc />
    public async Task<UsuarioResponse> CriarAsync(
        CriarUsuarioRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        await ExigirEmailDisponivelAsync(requisicao.Email, idIgnorado: null, cancellationToken);

        var perfil = await BuscarPerfilOuFalharAsync(requisicao.PerfilId, cancellationToken);

        var usuario = new Usuario(
            requisicao.Nome,
            requisicao.Email,
            hashDeSenha.GerarHash(requisicao.Senha),
            perfil.Id);

        await usuarios.AdicionarAsync(usuario, cancellationToken);
        await usuarios.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioResponse.De(await BuscarOuFalharAsync(usuario.Id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<UsuarioResponse> AtualizarAsync(
        int id,
        AtualizarUsuarioRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var usuario = await BuscarOuFalharAsync(id, cancellationToken);

        // O próprio usuário é desconsiderado na checagem: salvar o cadastro sem trocar o
        // e-mail não pode ser tratado como duplicidade dele mesmo.
        await ExigirEmailDisponivelAsync(requisicao.Email, idIgnorado: id, cancellationToken);

        usuario.AtualizarDados(requisicao.Nome, requisicao.Email);

        usuarios.Atualizar(usuario);
        await usuarios.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioResponse.De(usuario);
    }

    private async Task<Usuario> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await usuarios.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Usuário", id);

    private async Task<Perfil> BuscarPerfilOuFalharAsync(
        int perfilId,
        CancellationToken cancellationToken)
    {
        var perfil = await perfis.ObterPorIdAsync(perfilId, cancellationToken)
            ?? throw new NaoEncontradoException("Perfil de acesso", perfilId);

        if (!perfil.Ativo)
        {
            throw new RegraDeNegocioException(
                $"O perfil '{perfil.Nome}' está inativo e não pode ser atribuído.");
        }

        return perfil;
    }

    private async Task ExigirEmailDisponivelAsync(
        string email,
        int? idIgnorado,
        CancellationToken cancellationToken)
    {
        if (await usuarios.ExisteComEmailAsync(email, idIgnorado, cancellationToken))
        {
            throw new ConflitoException("usuário", "e-mail", email.Trim().ToLowerInvariant());
        }
    }
}
