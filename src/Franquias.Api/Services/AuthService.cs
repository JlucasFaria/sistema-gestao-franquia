using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Autenticacao;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IAuthService"/>.
/// </summary>
public sealed class AuthService(
    IRepositorio<Usuario> usuarios,
    IRepositorio<Perfil> perfis,
    IHashDeSenhaService hashDeSenha,
    ITokenService tokens) : IAuthService
{
    private const string MensagemDeCredencialInvalida = "E-mail ou senha inválidos.";

    /// <inheritdoc />
    public async Task<LoginResponse> AutenticarAsync(
        LoginRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizarEmail(requisicao.Email);

        var usuario = await usuarios
            .Consultar()
            .Include(candidato => candidato.Perfil)
            .FirstOrDefaultAsync(candidato => candidato.Email == email, cancellationToken);

        // A mesma mensagem para usuário inexistente e para senha incorreta: qualquer
        // diferença permitiria descobrir quais e-mails estão cadastrados.
        if (usuario is null || !hashDeSenha.Verificar(requisicao.Senha, usuario.SenhaHash))
        {
            throw new CredenciaisInvalidasException(MensagemDeCredencialInvalida);
        }

        // A conta inativa só é revelada depois de a senha conferir, para não expor a
        // situação de contas a quem não conhece a credencial.
        if (!usuario.Ativo)
        {
            throw new CredenciaisInvalidasException(
                "Este usuário está inativo. Procure o administrador da rede.");
        }

        return MontarResposta(usuario);
    }

    /// <inheritdoc />
    public async Task<LoginResponse> RegistrarAsync(
        RegistroRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizarEmail(requisicao.Email);

        if (await usuarios.ExisteAsync(candidato => candidato.Email == email, cancellationToken))
        {
            throw new ConflitoException("usuário", "e-mail", email);
        }

        var perfil = await perfis.ObterPorIdAsync(requisicao.PerfilId, cancellationToken)
            ?? throw new NaoEncontradoException("Perfil de acesso", requisicao.PerfilId);

        if (!perfil.Ativo)
        {
            throw new RegraDeNegocioException(
                $"O perfil '{perfil.Nome}' está inativo e não pode ser atribuído.");
        }

        var usuario = new Usuario(
            requisicao.Nome,
            email,
            hashDeSenha.GerarHash(requisicao.Senha),
            perfil.Id);

        await usuarios.AdicionarAsync(usuario, cancellationToken);
        await usuarios.SalvarAlteracoesAsync(cancellationToken);

        // Recarrega com o perfil incluído: a emissão do token depende dessa navegação.
        var persistido = await usuarios
            .Consultar()
            .Include(candidato => candidato.Perfil)
            .FirstAsync(candidato => candidato.Id == usuario.Id, cancellationToken);

        return MontarResposta(persistido);
    }

    private LoginResponse MontarResposta(Usuario usuario)
    {
        var token = tokens.Gerar(usuario);

        return new LoginResponse(
            token.Token,
            token.ExpiraEm,
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil.Nome);
    }

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
