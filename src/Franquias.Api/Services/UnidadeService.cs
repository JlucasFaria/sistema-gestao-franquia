using Franquias.Api.Common;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IUnidadeService"/>.
/// </summary>
public sealed class UnidadeService(
    IUnidadeRepositorio unidades,
    IRepositorio<Franqueadora> franqueadoras,
    IRepositorio<Franqueado> franqueados) : IUnidadeService
{
    /// <inheritdoc />
    public async Task<PagedResult<UnidadeResponse>> ListarAsync(
        QueryParams parametros,
        FiltroUnidadesRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var pagina = await unidades.ListarAsync(parametros, filtro, cancellationToken);

        return pagina.Converter(UnidadeResponse.De);
    }

    /// <inheritdoc />
    public async Task<UnidadeResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarOuFalharAsync(id, cancellationToken);

        return UnidadeResponse.De(unidade);
    }

    /// <inheritdoc />
    public async Task<UnidadeResponse> CriarAsync(
        CriarUnidadeRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var cnpj = requisicao.Cnpj.SomenteDigitos();

        if (await unidades.ExisteComCnpjAsync(cnpj, idIgnorado: null, cancellationToken))
        {
            throw new ConflitoException("unidade franqueada", "CNPJ", cnpj);
        }

        var franqueadora = await franqueadoras.ObterPorIdAsync(requisicao.FranqueadoraId, cancellationToken)
            ?? throw new NaoEncontradoException("Franqueadora", requisicao.FranqueadoraId);

        if (!franqueadora.Ativo)
        {
            throw new RegraDeNegocioException(
                $"A franqueadora '{franqueadora.NomeFantasia}' está inativa e não pode receber novas unidades.");
        }

        var franqueado = await franqueados.ObterPorIdAsync(requisicao.FranqueadoId, cancellationToken)
            ?? throw new NaoEncontradoException("Franqueado", requisicao.FranqueadoId);

        if (!franqueado.Ativo)
        {
            throw new RegraDeNegocioException(
                $"O franqueado '{franqueado.Nome}' está inativo e não pode assumir novas unidades.");
        }

        var unidade = new UnidadeFranqueada(
            franqueadora.Id,
            franqueado.Id,
            requisicao.RazaoSocial,
            requisicao.NomeFantasia,
            cnpj,
            requisicao.Email,
            requisicao.Telefone,
            requisicao.Endereco.ParaEntidade(),
            requisicao.DataInicio!.Value,
            requisicao.PercentualRoyalty);

        await unidades.AdicionarAsync(unidade, cancellationToken);
        await unidades.SalvarAlteracoesAsync(cancellationToken);

        return UnidadeResponse.De(await BuscarOuFalharAsync(unidade.Id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<UnidadeResponse> AtualizarAsync(
        int id,
        AtualizarUnidadeRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarOuFalharAsync(id, cancellationToken);

        unidade.Atualizar(
            requisicao.RazaoSocial,
            requisicao.NomeFantasia,
            requisicao.Email,
            requisicao.Telefone,
            requisicao.Endereco.ParaEntidade());

        // Entidade rastreada: a troca do endereço owned é detectada pelo change tracker.
        await unidades.SalvarAlteracoesAsync(cancellationToken);

        return UnidadeResponse.De(unidade);
    }

    /// <inheritdoc />
    public async Task<UnidadeResponse> AlterarSituacaoAsync(
        int id,
        SituacaoUnidade situacao,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarOuFalharAsync(id, cancellationToken);

        // Encerramento é o fim do contrato. Reabrir a mesma unidade apagaria da história o
        // encerramento e misturaria dois períodos contratuais distintos no mesmo cadastro;
        // uma nova operação no mesmo ponto deve ser cadastrada como nova unidade.
        if (unidade.Situacao == SituacaoUnidade.Encerrada && situacao != SituacaoUnidade.Encerrada)
        {
            throw new RegraDeNegocioException(
                "A unidade está encerrada e não pode mudar de situação. Uma nova operação no "
                + "mesmo ponto deve ser cadastrada como uma nova unidade.");
        }

        if (unidade.Situacao != situacao)
        {
            unidade.AlterarSituacao(situacao);
            await unidades.SalvarAlteracoesAsync(cancellationToken);
        }

        return UnidadeResponse.De(unidade);
    }

    /// <inheritdoc />
    public async Task<UnidadeResponse> AtivarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarOuFalharAsync(id, cancellationToken);

        unidade.Ativar();
        await unidades.SalvarAlteracoesAsync(cancellationToken);

        return UnidadeResponse.De(unidade);
    }

    /// <inheritdoc />
    public async Task<UnidadeResponse> InativarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var unidade = await BuscarOuFalharAsync(id, cancellationToken);

        unidade.Inativar();
        await unidades.SalvarAlteracoesAsync(cancellationToken);

        return UnidadeResponse.De(unidade);
    }

    private async Task<UnidadeFranqueada> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await unidades.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Unidade franqueada", id);
}
