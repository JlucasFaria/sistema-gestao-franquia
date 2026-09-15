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

        var autor = await usuarios.ObterPorIdAsync(usuarioAberturaId, cancellationToken)
            ?? throw new NaoEncontradoException("Usuário", usuarioAberturaId);

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

    private async Task<ChamadoSuporte> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await chamados.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Chamado de suporte", id);
}
