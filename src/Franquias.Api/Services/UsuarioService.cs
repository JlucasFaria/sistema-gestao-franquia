using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Usuarios;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
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

    /// <inheritdoc />
    public async Task<UsuarioResponse> AtivarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var usuario = await BuscarOuFalharAsync(id, cancellationToken);

        // Ativar() não faz nada se já estiver ativo, então repetir a chamada é inofensivo.
        usuario.Ativar();

        usuarios.Atualizar(usuario);
        await usuarios.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioResponse.De(usuario);
    }

    /// <inheritdoc />
    public async Task<UsuarioResponse> InativarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var usuario = await BuscarOuFalharAsync(id, cancellationToken);

        await ExigirQueSobreAdministradorAsync(usuario, cancellationToken);

        usuario.Inativar();

        usuarios.Atualizar(usuario);
        await usuarios.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioResponse.De(usuario);
    }

    /// <inheritdoc />
    public async Task<UsuarioResponse> VincularPerfilAsync(
        int id,
        int perfilId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await BuscarOuFalharAsync(id, cancellationToken);
        var perfil = await BuscarPerfilOuFalharAsync(perfilId, cancellationToken);

        // Rebaixar o último administrador tem o mesmo efeito prático de inativá-lo: a rede
        // fica sem ninguém capaz de administrá-la. A mesma trava vale para os dois casos.
        if (perfil.Codigo != PerfilAcesso.Administrador)
        {
            await ExigirQueSobreAdministradorAsync(usuario, cancellationToken);
        }

        usuario.VincularPerfil(perfil.Id);

        usuarios.Atualizar(usuario);
        await usuarios.SalvarAlteracoesAsync(cancellationToken);

        return UsuarioResponse.De(await BuscarOuFalharAsync(id, cancellationToken));
    }

    /// <summary>
    /// Impede que o último administrador ativo seja inativado. Sem essa trava, uma única
    /// requisição deixaria a rede sem ninguém capaz de cadastrar usuários ou reativar
    /// contas — e o bloqueio só seria reversível mexendo direto no banco.
    /// </summary>
    private async Task ExigirQueSobreAdministradorAsync(
        Usuario usuario,
        CancellationToken cancellationToken)
    {
        if (!usuario.Ativo || usuario.Perfil.Codigo != PerfilAcesso.Administrador)
        {
            return;
        }

        var administradoresAtivos = await usuarios.ContarAdministradoresAtivosAsync(cancellationToken);

        if (administradoresAtivos <= 1)
        {
            throw new RegraDeNegocioException(
                "Este é o único administrador ativo da rede. Cadastre ou ative outro "
                + "administrador antes de inativá-lo ou trocar o perfil dele.");
        }
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
