using Franquias.Api.Common;
using Franquias.Api.Entities.Enums;

namespace Franquias.Api.Entities;

/// <summary>
/// Loja da rede operada por um franqueado sob a marca da franqueadora.
/// </summary>
public class UnidadeFranqueada : EntidadeBase
{
    private readonly List<Responsavel> _responsaveis = [];

    /// <summary>Construtor exigido pelo Entity Framework Core.</summary>
    protected UnidadeFranqueada()
    {
    }

    public UnidadeFranqueada(
        int franqueadoraId,
        int franqueadoId,
        string razaoSocial,
        string nomeFantasia,
        string cnpj,
        string email,
        string telefone,
        Endereco endereco,
        DateOnly dataInicio,
        decimal percentualRoyalty)
    {
        FranqueadoraId = franqueadoraId;
        FranqueadoId = franqueadoId;
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Cnpj = cnpj.SomenteDigitos();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        Endereco = endereco;
        DataInicio = dataInicio;
        PercentualRoyalty = percentualRoyalty;
        Situacao = SituacaoUnidade.EmImplantacao;
    }

    /// <summary>Chave estrangeira da franqueadora dona da marca.</summary>
    public int FranqueadoraId { get; private set; }

    /// <summary>Franqueadora dona da marca.</summary>
    public Franqueadora Franqueadora { get; private set; } = null!;

    /// <summary>Chave estrangeira do franqueado que opera a unidade.</summary>
    public int FranqueadoId { get; private set; }

    /// <summary>Franqueado que opera a unidade.</summary>
    public Franqueado Franqueado { get; private set; } = null!;

    /// <summary>Razão social registrada da unidade.</summary>
    public string RazaoSocial { get; private set; } = string.Empty;

    /// <summary>Nome pelo qual a unidade é conhecida.</summary>
    public string NomeFantasia { get; private set; } = string.Empty;

    /// <summary>CNPJ gravado apenas com dígitos. Único em toda a base.</summary>
    public string Cnpj { get; private set; } = string.Empty;

    /// <summary>E-mail de contato da unidade.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Telefone de contato gravado apenas com dígitos.</summary>
    public string Telefone { get; private set; } = string.Empty;

    /// <summary>Endereço da unidade.</summary>
    public Endereco Endereco { get; private set; } = null!;

    /// <summary>Data de início da operação prevista em contrato.</summary>
    public DateOnly DataInicio { get; private set; }

    /// <summary>Situação contratual da unidade.</summary>
    public SituacaoUnidade Situacao { get; private set; }

    /// <summary>Percentual de royalty aplicado sobre o faturamento do período.</summary>
    public decimal PercentualRoyalty { get; private set; }

    /// <summary>Pessoas responsáveis pela operação da unidade.</summary>
    public IReadOnlyCollection<Responsavel> Responsaveis => _responsaveis;

    /// <summary>
    /// Indica se a unidade está apta a operar. É a condição verificada antes de registrar
    /// vendas e movimentações de estoque.
    /// </summary>
    public bool PodeOperar() => Ativo && Situacao == SituacaoUnidade.Ativa;

    /// <summary>
    /// Atualiza os dados cadastrais da unidade. O CNPJ é imutável.
    /// </summary>
    public void Atualizar(
        string razaoSocial,
        string nomeFantasia,
        string email,
        string telefone,
        Endereco endereco)
    {
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.SomenteDigitos();
        Endereco = endereco;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Altera a situação contratual da unidade.
    /// </summary>
    public void AlterarSituacao(SituacaoUnidade situacao)
    {
        Situacao = situacao;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Define o percentual de royalty cobrado da unidade. A faixa aceita é validada pelo
    /// serviço de royalties antes da chamada.
    /// </summary>
    public void DefinirPercentualRoyalty(decimal percentual)
    {
        PercentualRoyalty = percentual;
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Vincula um responsável à unidade.
    /// </summary>
    public void AdicionarResponsavel(Responsavel responsavel)
    {
        _responsaveis.Add(responsavel);
        RegistrarAtualizacao();
    }

    /// <summary>
    /// Desvincula um responsável da unidade.
    /// </summary>
    public void RemoverResponsavel(Responsavel responsavel)
    {
        _responsaveis.Remove(responsavel);
        RegistrarAtualizacao();
    }
}
