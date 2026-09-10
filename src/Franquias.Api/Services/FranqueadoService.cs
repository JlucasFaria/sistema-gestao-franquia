using Franquias.Api.Common;
using Franquias.Api.Common.Consultas;
using Franquias.Api.Common.Excecoes;
using Franquias.Api.DTOs.Franqueados;
using Franquias.Api.Entities;
using Franquias.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services;

/// <summary>
/// Implementação de <see cref="IFranqueadoService"/>.
/// </summary>
public sealed class FranqueadoService(IRepositorio<Franqueado> franqueados) : IFranqueadoService
{
    /// <inheritdoc />
    public async Task<PagedResult<FranqueadoResponse>> ListarAsync(
        QueryParams parametros,
        CancellationToken cancellationToken = default)
    {
        var consulta = franqueados.Consultar();

        if (!string.IsNullOrWhiteSpace(parametros.Busca))
        {
            var termo = $"%{parametros.Busca.Trim()}%";
            var digitos = parametros.Busca.SomenteDigitos();

            // O CPF só entra na busca quando o termo tem dígitos: sem essa condição, um
            // termo como "Ana" viraria um LIKE '%%' no CPF e casaria com todos os registros.
            consulta = consulta.Where(franqueado =>
                EF.Functions.Like(franqueado.Nome, termo)
                || (digitos != string.Empty && franqueado.Cpf.Contains(digitos)));
        }

        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta.OrderBy(franqueado => franqueado.Nome)
            : consulta.Ordenar(parametros);

        var pagina = await ordenada.PaginarAsync(parametros, cancellationToken);

        return pagina.Converter(FranqueadoResponse.De);
    }

    /// <inheritdoc />
    public async Task<FranqueadoResponse> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var franqueado = await BuscarOuFalharAsync(id, cancellationToken);

        return FranqueadoResponse.De(franqueado);
    }

    /// <inheritdoc />
    public async Task<FranqueadoResponse> CriarAsync(
        CriarFranqueadoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var cpf = requisicao.Cpf.SomenteDigitos();

        if (await franqueados.ExisteAsync(franqueado => franqueado.Cpf == cpf, cancellationToken))
        {
            throw new ConflitoException("franqueado", "CPF", cpf);
        }

        var novo = new Franqueado(
            requisicao.Nome,
            cpf,
            requisicao.Email,
            requisicao.Telefone,
            requisicao.DataAdesao!.Value);

        await franqueados.AdicionarAsync(novo, cancellationToken);
        await franqueados.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoResponse.De(novo);
    }

    /// <inheritdoc />
    public async Task<FranqueadoResponse> AtualizarAsync(
        int id,
        AtualizarFranqueadoRequest requisicao,
        CancellationToken cancellationToken = default)
    {
        var franqueado = await BuscarOuFalharAsync(id, cancellationToken);

        franqueado.Atualizar(requisicao.Nome, requisicao.Email, requisicao.Telefone);
        await franqueados.SalvarAlteracoesAsync(cancellationToken);

        return FranqueadoResponse.De(franqueado);
    }

    private async Task<Franqueado> BuscarOuFalharAsync(int id, CancellationToken cancellationToken) =>
        await franqueados.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NaoEncontradoException("Franqueado", id);
}
