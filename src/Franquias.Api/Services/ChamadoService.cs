using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Chamados;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IChamadoService"/>.
/// </summary>
public sealed class ChamadoService(
    IChamadoRepositorio chamados,
    IUnidadeRepositorio unidades,
    IUsuarioRepositorio usuarios) : IChamadoService
{
    /// <inheritdoc />
    public async Task<ChamadoResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var chamado = await BuscarOuFalharAsync(id, cancellationToken);

        return ChamadoResponse.De(chamado);
    }

    /// <inheritdoc />
    public async Task<PagedResult<ChamadoResponse>> ListarAsync(
        QueryParams parametros,
        FiltroChamadosRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var pagina = await chamados.ListarAsync(parametros, filtro, cancellationToken);

        return pagina.Converter(ChamadoResponse.De);
    }

    /// <inheritdoc />
    public async Task<ChamadoResponse> AbrirAsync(
        AbrirChamadoRequest requisicao,
        int usuarioAberturaId,
        CancellationToken cancellationToken = default)
    {
        var unidade = await unidades.ObterPorIdAsync(requisicao.UnidadeFranqueadaId, cancellationToken)
            ?? throw new NaoEncontradoException("Unidade franqueada", requisicao.UnidadeFranqueadaId);

        // Uma unidade inativa saiu da rede: seu histórico é preservado, mas ela não abre
        // novos chamados. A unidade em implantação abre, pois é quando mais precisa de apoio.
        if (!unidade.Ativo)
        {
            throw new RegraDeNegocioException(
                $"A unidade '{unidade.NomeFantasia}' está inativa e não pode abrir chamados.");
        }

        var autor = await BuscarAutorOuFalharAsync(usuarioAberturaId, cancellationToken);

        var chamado = new ChamadoSuporte(
            unidade.Id,
            autor.Id,
            requisicao.Categoria.Trim(),
            requisicao.Assunto.Trim(),
            requisicao.Descricao.Trim(),
            requisicao.Prioridade!.Value);

        await chamados.AdicionarAsync(chamado, cancellationToken);
        await chamados.SalvarAlteracoesAsync(cancellationToken);

        return ChamadoResponse.De(await BuscarOuFalharAsync(chamado.Id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<ChamadoResponse> AlterarPrioridadeAsync(
        int id,
        AlterarPrioridadeChamadoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var chamado = await BuscarOuFalharAsync(id, cancellationToken);

        ExigirChamadoEmAberto(chamado);

        var prioridade = requisicao.Prioridade!.Value;

        // Repetir a prioridade atual não é erro, mas também não merece um registro de
        // atualização no histórico do chamado.
        if (chamado.Prioridade != prioridade)
        {
            chamado.AlterarPrioridade(prioridade);
            await chamados.SalvarAlteracoesAsync(cancellationToken);
        }

        return ChamadoResponse.De(chamado);
    }

    /// <inheritdoc />
    public async Task<ChamadoResponse> RegistrarInteracaoAsync(
        int id,
        RegistrarInteracaoRequest requisicao,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        var chamado = await BuscarOuFalharAsync(id, cancellationToken);

        ExigirChamadoEmAberto(chamado);

        var autor = await BuscarAutorOuFalharAsync(usuarioId, cancellationToken);

        chamado.RegistrarInteracao(autor.Id, requisicao.Mensagem.Trim());
        await chamados.SalvarAlteracoesAsync(cancellationToken);

        // Relê o chamado para que a mensagem recém-gravada volte com o nome do autor.
        return ChamadoResponse.De(await BuscarOuFalharAsync(chamado.Id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<ChamadoResponse> AlterarStatusAsync(
        int id,
        AlterarStatusChamadoRequest requisicao,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        var chamado = await BuscarOuFalharAsync(id, cancellationToken);

        ExigirChamadoEmAberto(chamado);

        var status = requisicao.Status!.Value;
        var observacao = requisicao.Observacao?.Trim();

        if (chamado.Status == status && string.IsNullOrEmpty(observacao))
        {
            return ChamadoResponse.De(chamado);
        }

        var autor = await BuscarAutorOuFalharAsync(usuarioId, cancellationToken);

        // A observação entra antes da mudança: depois de encerrado, o chamado não aceita
        // mais nenhum registro.
        if (!string.IsNullOrEmpty(observacao))
        {
            chamado.RegistrarInteracao(autor.Id, observacao);
        }

        if (chamado.Status != status)
        {
            chamado.AlterarStatus(status);
        }

        await chamados.SalvarAlteracoesAsync(cancellationToken);

        return ChamadoResponse.De(await BuscarOuFalharAsync(chamado.Id, cancellationToken));
    }

    /// <summary>
    /// A entidade também recusa mexer em chamado encerrado, mas com uma exceção técnica.
    /// Aqui a recusa vira resposta tratada.
    /// </summary>
    private static void ExigirChamadoEmAberto(ChamadoSuporte chamado)
    {
        if (!chamado.EstaEmAberto())
        {
            throw new RegraDeNegocioException(
                $"O chamado {chamado.Id} foi encerrado e não aceita novas alterações.");
        }
    }

    private async Task<Usuario> BuscarAutorOuFalharAsync(int usuarioId, CancellationToken cancellationToken) =>
        await usuarios.ObterPorIdAsync(usuarioId, cancellationToken)
            ?? throw new NaoEncontradoException("Usuário", usuarioId);

    private async Task<ChamadoSuporte> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await chamados.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Chamado de suporte", id);
}
