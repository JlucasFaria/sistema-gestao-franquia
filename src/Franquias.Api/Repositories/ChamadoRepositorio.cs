using Franquias.Api.Common.Consultas;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Chamados;
using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

/// <summary>
/// Implementação de <see cref="IChamadoRepositorio"/>.
/// </summary>
public class ChamadoRepositorio(AppDbContext contexto)
    : RepositorioGenerico<ChamadoSuporte>(contexto), IChamadoRepositorio
{
    /// <summary>
    /// Sobrescreve a busca por identificador para trazer a unidade, o autor da abertura e a
    /// linha do tempo: registrar interação e encerrar o chamado dependem desse histórico.
    /// </summary>
    public override async Task<ChamadoSuporte?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await ComRelacionamentos(Conjunto)
            .FirstOrDefaultAsync(chamado => chamado.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<PagedResult<ChamadoSuporte>> ListarAsync(
        QueryParams parametros,
        FiltroChamadosRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var consulta = Filtrar(ComRelacionamentos(Conjunto.AsNoTracking()), filtro);

        // Sem ordenação pedida, o chamado mais recente primeiro. O desempate pelo
        // identificador mantém a paginação estável.
        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta
                .OrderByDescending(chamado => chamado.DataCriacao)
                .ThenByDescending(chamado => chamado.Id)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }

    /// <summary>
    /// Aplica os filtros informados. Cada um é opcional e se soma aos demais.
    /// </summary>
    protected static IQueryable<ChamadoSuporte> Filtrar(
        IQueryable<ChamadoSuporte> consulta,
        FiltroChamadosRequest filtro)
    {
        if (filtro.UnidadeFranqueadaId is not null)
        {
            consulta = consulta.Where(chamado => chamado.UnidadeFranqueadaId == filtro.UnidadeFranqueadaId);
        }

        if (filtro.Status is not null)
        {
            consulta = consulta.Where(chamado => chamado.Status == filtro.Status);
        }

        if (filtro.Prioridade is not null)
        {
            consulta = consulta.Where(chamado => chamado.Prioridade == filtro.Prioridade);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
        {
            var categoria = filtro.Categoria.Trim();

            consulta = consulta.Where(chamado => EF.Functions.Like(chamado.Categoria, categoria));
        }

        if (filtro.SomenteEmAberto == true)
        {
            consulta = consulta.Where(chamado => chamado.Status != StatusChamado.Encerrado);
        }

        return consulta;
    }

    /// <summary>
    /// Carrega a unidade, o autor da abertura e a linha do tempo com os autores das mensagens.
    /// </summary>
    protected static IQueryable<ChamadoSuporte> ComRelacionamentos(IQueryable<ChamadoSuporte> consulta) =>
        consulta
            .Include(chamado => chamado.UnidadeFranqueada)
            .Include(chamado => chamado.UsuarioAbertura)
            .Include(chamado => chamado.Interacoes)
                .ThenInclude(interacao => interacao.Usuario);

    /// <inheritdoc />
    /// <remarks>
    /// A ordem reproduz a fila de atendimento do suporte: primeiro o que trava a operação da
    /// unidade, e entre iguais o que espera há mais tempo. Uma ordenação pedida na consulta
    /// tem preferência sobre essa.
    /// </remarks>
    public async Task<PagedResult<ChamadoSuporte>> ListarEmAbertoAsync(
        QueryParams parametros,
        FiltroChamadosRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var consulta = Filtrar(ComRelacionamentos(Conjunto.AsNoTracking()), filtro)
            .Where(chamado => chamado.Status != StatusChamado.Encerrado);

        var ordenada = string.IsNullOrWhiteSpace(parametros.OrdenarPor)
            ? consulta
                .OrderByDescending(chamado => chamado.Prioridade)
                .ThenBy(chamado => chamado.DataCriacao)
                .ThenBy(chamado => chamado.Id)
            : consulta.Ordenar(parametros);

        return await ordenada.PaginarAsync(parametros, cancellationToken);
    }
}
