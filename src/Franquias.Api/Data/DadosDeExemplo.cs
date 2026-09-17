using Franquias.Api.Entities;
using Franquias.Api.Entities.Enums;
using Franquias.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

/// <summary>
/// Carga de dados de demonstração: unidades, produtos, fornecedores, estoque, vendas,
/// royalties e chamados suficientes para exercitar filtros, cálculos e relatórios logo
/// após o primeiro <c>dotnet run</c>.
/// </summary>
/// <remarks>
/// As datas são relativas ao dia da carga: as vendas cobrem os três últimos meses fechados
/// e o mês corrente, de modo que os relatórios por período e as cobranças de royalty façam
/// sentido em qualquer dia em que o banco for criado. O sorteio usa semente fixa, então a
/// mesma data gera sempre os mesmos dados.
/// </remarks>
public static class DadosDeExemplo
{
    private const int FranqueadoraId = 1;
    private const int AdministradorId = 1;
    private const int PerfilGestor = 2;
    private const int PerfilOperador = 3;
    private const int CategoriaAlimentos = 1;
    private const int CategoriaBebidas = 2;
    private const int CategoriaInsumos = 3;
    private const int CategoriaServicos = 4;

    /// <summary>
    /// Carrega os dados de exemplo em um banco que ainda não tem unidades cadastradas.
    /// Um banco já em uso nunca é tocado.
    /// </summary>
    /// <returns>Verdadeiro se a carga foi feita; falso se o banco já tinha dados.</returns>
    public static async Task<bool> CarregarAsync(
        AppDbContext contexto,
        IHashDeSenhaService hashDeSenha,
        CancellationToken cancellationToken = default)
    {
        if (await contexto.Unidades.AnyAsync(cancellationToken))
        {
            return false;
        }

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var sorteio = new Random(2026);

        // Tudo ou nada: uma falha no meio não deixa o banco com metade da demonstração.
        await using var transacao = await contexto.Database.BeginTransactionAsync(cancellationToken);

        var (gestor, operador) = CriarUsuarios(contexto, hashDeSenha);
        var unidades = await CriarRedeAsync(contexto, cancellationToken);
        var produtos = CriarCatalogo(contexto);
        await contexto.SaveChangesAsync(cancellationToken);

        CriarFornecedores(contexto, produtos);
        var estoques = CriarEstoques(contexto, unidades, produtos);
        await contexto.SaveChangesAsync(cancellationToken);

        var vendas = await CriarVendasAsync(contexto, unidades, produtos, estoques, hoje, sorteio, cancellationToken);
        CriarRoyalties(contexto, unidades, vendas, hoje);
        AjustarInventario(unidades, produtos, estoques);
        await contexto.SaveChangesAsync(cancellationToken);

        await CriarChamadosAsync(contexto, unidades, gestor, operador, hoje, cancellationToken);

        await transacao.CommitAsync(cancellationToken);

        return true;
    }

    private static (Usuario Gestor, Usuario Operador) CriarUsuarios(
        AppDbContext contexto,
        IHashDeSenhaService hashDeSenha)
    {
        var gestor = new Usuario("Marina Costa", "gestor@franquias.com.br", hashDeSenha.GerarHash("Gestor@123"), PerfilGestor);
        var operador = new Usuario("Rafael Lima", "operador@franquias.com.br", hashDeSenha.GerarHash("Operador@123"), PerfilOperador);

        contexto.Usuarios.AddRange(gestor, operador);

        return (gestor, operador);
    }

    /// <summary>
    /// Três franqueados e cinco unidades em situações diferentes: três em operação, uma em
    /// implantação e uma encerrada e inativada, para demonstrar a listagem por situação.
    /// </summary>
    private static async Task<List<UnidadeFranqueada>> CriarRedeAsync(
        AppDbContext contexto,
        CancellationToken cancellationToken)
    {
        var ana = new Franqueado("Ana Beatriz Souza", "11144477735", "ana.souza@email.com", "11987654321", new DateOnly(2024, 2, 1));
        var bruno = new Franqueado("Bruno Henrique Alves", "22255588846", "bruno.alves@email.com", "31987654321", new DateOnly(2024, 8, 15));
        var carla = new Franqueado("Carla Mendes Rocha", "33366699957", "carla.rocha@email.com", "41987654321", new DateOnly(2025, 5, 10));

        // Gravados antes das unidades, que dependem dos seus identificadores.
        contexto.Franqueados.AddRange(ana, bruno, carla);
        await contexto.SaveChangesAsync(cancellationToken);

        var paulista = NovaUnidade(ana, "Paulista", "34567890000130", "Av. Paulista", "1500", "Bela Vista", "São Paulo", "SP", "01310200", new DateOnly(2024, 3, 1), 5m);
        var campinas = NovaUnidade(ana, "Campinas", "45678901000175", "Rua Barão de Jaguara", "820", "Centro", "Campinas", "SP", "13015001", new DateOnly(2024, 9, 1), 6m);
        var savassi = NovaUnidade(bruno, "Savassi", "56789012000100", "Rua Pernambuco", "1000", "Savassi", "Belo Horizonte", "MG", "30130151", new DateOnly(2024, 10, 1), 5.5m);
        var curitiba = NovaUnidade(carla, "Curitiba", "67890123000116", "Rua XV de Novembro", "300", "Centro", "Curitiba", "PR", "80020310", new DateOnly(2026, 8, 1), 5m);
        var niteroi = NovaUnidade(carla, "Niterói", "78901234000105", "Rua Moreira César", "150", "Icaraí", "Niterói", "RJ", "24230060", new DateOnly(2025, 6, 1), 5m);

        paulista.AlterarSituacao(SituacaoUnidade.Ativa);
        campinas.AlterarSituacao(SituacaoUnidade.Ativa);
        savassi.AlterarSituacao(SituacaoUnidade.Ativa);
        niteroi.AlterarSituacao(SituacaoUnidade.Encerrada);
        niteroi.Inativar();

        paulista.AdicionarResponsavel(new Responsavel("Lucas Pereira", "14725836982", "Gerente", "lucas.pereira@email.com", "11912345678", 0));
        campinas.AdicionarResponsavel(new Responsavel("Fernanda Dias", "25836914737", "Gerente", "fernanda.dias@email.com", "19912345678", 0));
        savassi.AdicionarResponsavel(new Responsavel("Gustavo Ramos", "36914725837", "Gerente", "gustavo.ramos@email.com", "31912345678", 0));
        curitiba.AdicionarResponsavel(new Responsavel("Juliana Martins", "74185296355", "Coordenadora de implantação", "juliana.martins@email.com", "41912345678", 0));
        niteroi.AdicionarResponsavel(new Responsavel("Pedro Nogueira", "85296374100", "Gerente", "pedro.nogueira@email.com", "21912345678", 0));

        List<UnidadeFranqueada> unidades = [paulista, campinas, savassi, curitiba, niteroi];
        contexto.Unidades.AddRange(unidades);

        return unidades;
    }

    private static UnidadeFranqueada NovaUnidade(
        Franqueado franqueado,
        string bairroDaMarca,
        string cnpj,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string uf,
        string cep,
        DateOnly dataInicio,
        decimal percentualRoyalty)
    {
        return new UnidadeFranqueada(
            FranqueadoraId,
            franqueado.Id,
            $"Sabor Brasil {bairroDaMarca} Alimentos LTDA",
            $"Sabor Brasil {bairroDaMarca}",
            cnpj,
            $"{bairroDaMarca.ToLowerInvariant().Replace("ó", "o")}@saborbrasil.com.br",
            "1130004000",
            new Endereco(logradouro, numero, null, bairro, cidade, uf, cep),
            dataInicio,
            percentualRoyalty);
    }

    private static Dictionary<string, ProdutoServico> CriarCatalogo(AppDbContext contexto)
    {
        var produtos = new Dictionary<string, ProdutoServico>
        {
            ["coxinha"] = new(CategoriaAlimentos, "Coxinha de Frango", "Coxinha tradicional de frango com catupiry.", 8.90m, false),
            ["paoDeQueijo"] = new(CategoriaAlimentos, "Pão de Queijo", "Porção com seis unidades.", 6.50m, false),
            ["pastel"] = new(CategoriaAlimentos, "Pastel de Carne", "Pastel frito de carne moída.", 9.90m, false),
            ["suco"] = new(CategoriaBebidas, "Suco de Laranja 300ml", "Suco natural feito na hora.", 10.90m, false),
            ["cafe"] = new(CategoriaBebidas, "Café Expresso", "Café expresso de 50ml.", 5.50m, false),
            ["refrigerante"] = new(CategoriaBebidas, "Refrigerante Lata", "Lata de 350ml.", 6.00m, false),
            ["farinha"] = new(CategoriaInsumos, "Farinha de Trigo 5kg", "Insumo de produção.", 32.00m, false),
            ["oleo"] = new(CategoriaInsumos, "Óleo de Soja 900ml", "Insumo de produção.", 9.50m, false),
            ["consultoria"] = new(CategoriaServicos, "Consultoria de Layout", "Visita técnica para reorganizar o salão.", 350.00m, true),
            ["treinamento"] = new(CategoriaServicos, "Treinamento de Equipe", "Capacitação de atendimento com duração de 8 horas.", 480.00m, true),
            ["esfiha"] = new(CategoriaAlimentos, "Esfiha de Queijo", "Item retirado do cardápio.", 7.90m, false)
        };

        produtos["esfiha"].AlterarStatus(StatusProduto.Descontinuado);

        contexto.Produtos.AddRange(produtos.Values);

        return produtos;
    }

    /// <summary>
    /// Três fornecedores homologados para parte do catálogo. O último foi descredenciado,
    /// para demonstrar a consulta por status.
    /// </summary>
    private static void CriarFornecedores(AppDbContext contexto, Dictionary<string, ProdutoServico> produtos)
    {
        var alimentos = new Fornecedor("Distribuidora Paulista de Alimentos LTDA", "Paulista Alimentos", "89012345000179", "vendas@paulistaalimentos.com.br", "1133224455", new Endereco("Rua do Gasômetro", "400", null, "Brás", "São Paulo", "SP", "03004000"));
        var bebidas = new Fornecedor("Bebidas Vale do Paraíba LTDA", "Vale Bebidas", "90123456000131", "comercial@valebebidas.com.br", "1239214455", new Endereco("Av. Andrômeda", "2000", "Galpão 3", "Jardim Satélite", "São José dos Campos", "SP", "12230000"));
        var insumos = new Fornecedor("Moinho Sul Insumos LTDA", "Moinho Sul", "13579246000101", "contato@moinhosul.com.br", "4133224455", new Endereco("Rua Mateus Leme", "3500", null, "São Lourenço", "Curitiba", "PR", "82200000"));

        contexto.Fornecedores.AddRange(alimentos, bebidas, insumos);

        alimentos.AssociarProduto(new FornecedorProduto(0, produtos["coxinha"].Id, 3.20m, 2));
        alimentos.AssociarProduto(new FornecedorProduto(0, produtos["paoDeQueijo"].Id, 2.10m, 2));
        alimentos.AssociarProduto(new FornecedorProduto(0, produtos["pastel"].Id, 3.60m, 3));
        bebidas.AssociarProduto(new FornecedorProduto(0, produtos["suco"].Id, 4.50m, 1));
        bebidas.AssociarProduto(new FornecedorProduto(0, produtos["refrigerante"].Id, 2.80m, 5));
        bebidas.AssociarProduto(new FornecedorProduto(0, produtos["cafe"].Id, 1.20m, 7));
        insumos.AssociarProduto(new FornecedorProduto(0, produtos["farinha"].Id, 24.00m, 10));
        insumos.AssociarProduto(new FornecedorProduto(0, produtos["oleo"].Id, 6.90m, 10));

        insumos.Inativar();
    }

    /// <summary>
    /// Estoque inicial das unidades em operação e da unidade em implantação. Algumas
    /// quantidades foram escolhidas para que o relatório de estoque crítico tenha conteúdo.
    /// </summary>
    private static Dictionary<(int Unidade, int Produto), Estoque> CriarEstoques(
        AppDbContext contexto,
        List<UnidadeFranqueada> unidades,
        Dictionary<string, ProdutoServico> produtos)
    {
        var estoques = new Dictionary<(int, int), Estoque>();

        foreach (var unidade in unidades.Where(unidade => unidade.PodeOperar()))
        {
            foreach (var (chave, produto) in produtos.Where(par => par.Value.ControlaEstoque() && par.Value.EstaDisponivelParaVenda()))
            {
                var ehInsumo = produto.CategoriaId == CategoriaInsumos;
                var estoque = new Estoque(unidade.Id, produto.Id, ehInsumo ? 10 : 40);

                // A unidade de Campinas recebeu pouco suco: vai terminar o período abaixo do mínimo.
                var entrada = (ehInsumo, unidade.NomeFantasia, chave) switch
                {
                    (true, _, _) => 30,
                    (_, "Sabor Brasil Campinas", "suco") => 120,
                    _ => 500
                };

                estoque.RegistrarEntrada(entrada, "Estoque inicial da unidade");
                estoques[(unidade.Id, produto.Id)] = estoque;
            }
        }

        // A unidade em implantação já recebeu o primeiro lote, ainda abaixo do mínimo.
        var curitiba = unidades.Single(unidade => unidade.Situacao == SituacaoUnidade.EmImplantacao);
        var lote = new Estoque(curitiba.Id, produtos["coxinha"].Id, 30);
        lote.RegistrarEntrada(12, "Primeiro lote da implantação");
        estoques[(curitiba.Id, produtos["coxinha"].Id)] = lote;

        contexto.Estoques.AddRange(estoques.Values);

        return estoques;
    }

    /// <summary>
    /// Vendas diárias das unidades em operação, do início do terceiro mês fechado até ontem.
    /// As vendas confirmadas baixam o estoque exatamente como o serviço de vendas faria; as
    /// dos últimos dias ficam em parte pendentes, e algumas pendentes foram canceladas.
    /// </summary>
    private static async Task<List<Venda>> CriarVendasAsync(
        AppDbContext contexto,
        List<UnidadeFranqueada> unidades,
        Dictionary<string, ProdutoServico> produtos,
        Dictionary<(int Unidade, int Produto), Estoque> estoques,
        DateOnly hoje,
        Random sorteio,
        CancellationToken cancellationToken)
    {
        string[] cardapio = ["coxinha", "paoDeQueijo", "pastel", "suco", "cafe", "refrigerante"];
        var inicio = PrimeiroDiaDoMes(hoje).AddMonths(-3);
        var operando = unidades.Where(unidade => unidade.PodeOperar()).ToList();

        // Saldo simulado: impede sortear um item que a unidade não teria como entregar.
        var saldo = estoques.ToDictionary(par => par.Key, par => par.Value.Quantidade);
        var vendas = new List<Venda>();

        for (var dia = inicio; dia < hoje; dia = dia.AddDays(1))
        {
            foreach (var unidade in operando)
            {
                var quantidadeDeVendas = sorteio.Next(0, 3);

                for (var n = 0; n < quantidadeDeVendas; n++)
                {
                    var venda = new Venda(unidade.Id);
                    var itens = cardapio.OrderBy(_ => sorteio.Next()).Take(sorteio.Next(1, 4));

                    foreach (var chave in itens)
                    {
                        var produto = produtos[chave];
                        var quantidade = sorteio.Next(1, 5);

                        if (saldo[(unidade.Id, produto.Id)] < quantidade)
                        {
                            continue;
                        }

                        saldo[(unidade.Id, produto.Id)] -= quantidade;
                        venda.AdicionarItem(produto.Id, quantidade, produto.PrecoBase);
                    }

                    if (sorteio.NextDouble() < 0.02)
                    {
                        venda.AdicionarItem(produtos["consultoria"].Id, 1, produtos["consultoria"].PrecoBase);
                    }

                    if (venda.Itens.Count == 0)
                    {
                        continue;
                    }

                    var recente = hoje.DayNumber - dia.DayNumber <= 3;
                    var ficaPendente = recente && sorteio.NextDouble() < 0.5;

                    if (ficaPendente)
                    {
                        // A venda pendente não baixou estoque: devolve o saldo reservado.
                        foreach (var item in venda.Itens)
                        {
                            if (saldo.ContainsKey((unidade.Id, item.ProdutoServicoId)))
                            {
                                saldo[(unidade.Id, item.ProdutoServicoId)] += item.Quantidade;
                            }
                        }
                    }
                    else
                    {
                        venda.Confirmar();
                    }

                    contexto.Vendas.Add(venda);
                    contexto.Entry(venda).Property(v => v.DataVenda).CurrentValue =
                        dia.ToDateTime(new TimeOnly(sorteio.Next(9, 21), sorteio.Next(0, 60)), DateTimeKind.Utc);
                    vendas.Add(venda);
                }
            }
        }

        await contexto.SaveChangesAsync(cancellationToken);

        var confirmadas = vendas
            .Where(venda => venda.Status == StatusVenda.Confirmada)
            .OrderBy(venda => venda.DataVenda)
            .ToList();

        foreach (var (venda, posicao) in confirmadas.Select((venda, posicao) => (venda, posicao)))
        {
            foreach (var item in venda.Itens)
            {
                if (estoques.TryGetValue((venda.UnidadeFranqueadaId, item.ProdutoServicoId), out var estoque))
                {
                    estoque.RegistrarSaida(item.Quantidade, $"Venda {venda.Id}");
                }
            }

            // Uma a cada quarenta vendas foi cancelada depois de confirmada, com o estorno do
            // estoque registrado como faria o serviço de vendas.
            if (posicao % 40 != 20)
            {
                continue;
            }

            venda.Cancelar();

            foreach (var item in venda.Itens)
            {
                if (estoques.TryGetValue((venda.UnidadeFranqueadaId, item.ProdutoServicoId), out var estoque))
                {
                    estoque.RegistrarEntrada(item.Quantidade, $"Estorno da venda {venda.Id}");
                }
            }
        }

        // O registro da venda nasce com a data em que ela aconteceu, e não com a data da carga.
        await contexto.Vendas.ExecuteUpdateAsync(
            definicao => definicao.SetProperty(venda => venda.DataCriacao, venda => venda.DataVenda),
            cancellationToken);

        return vendas;
    }

    /// <summary>
    /// Cobranças dos três últimos meses fechados de cada unidade em operação, calculadas
    /// sobre o faturamento confirmado do mês, como faria o serviço de royalties. A mais
    /// antiga está paga; a do meio foi paga só pela primeira unidade, com juros, e está em
    /// atraso nas demais; a mais recente vence no dia 20 do mês corrente.
    /// </summary>
    private static void CriarRoyalties(
        AppDbContext contexto,
        List<UnidadeFranqueada> unidades,
        List<Venda> vendas,
        DateOnly hoje)
    {
        var mesCorrente = PrimeiroDiaDoMes(hoje);

        foreach (var (unidade, posicao) in unidades.Where(unidade => unidade.PodeOperar()).Select((unidade, posicao) => (unidade, posicao)))
        {
            for (var mesesAtras = 3; mesesAtras >= 1; mesesAtras--)
            {
                var inicio = mesCorrente.AddMonths(-mesesAtras);
                var fim = inicio.AddMonths(1).AddDays(-1);
                var vencimento = mesesAtras == 1 ? mesCorrente.AddDays(19) : fim.AddDays(10);

                var faturamento = vendas
                    .Where(venda => venda.UnidadeFranqueadaId == unidade.Id
                        && venda.Status == StatusVenda.Confirmada
                        && DateOnly.FromDateTime(venda.DataVenda) >= inicio
                        && DateOnly.FromDateTime(venda.DataVenda) <= fim)
                    .Sum(venda => venda.ValorTotal);

                var royalty = new Royalty(unidade.Id, inicio, fim, faturamento, unidade.PercentualRoyalty, vencimento);

                if (royalty.ValorDevido == decimal.Zero)
                {
                    continue;
                }

                if (mesesAtras == 3)
                {
                    royalty.RegistrarPagamento(vencimento.AddDays(-2), royalty.ValorDevido);
                }
                else if (mesesAtras == 2 && posicao == 0)
                {
                    var comJuros = Math.Round(royalty.ValorDevido * 1.02m, 2, MidpointRounding.AwayFromZero);
                    royalty.RegistrarPagamento(vencimento.AddDays(2), comJuros);
                }

                royalty.AvaliarAtraso(hoje);
                contexto.Royalties.Add(royalty);
            }
        }
    }

    /// <summary>
    /// Um inventário encontrou avaria no óleo da unidade Savassi, deixando o item abaixo
    /// do mínimo e registrando uma movimentação de ajuste.
    /// </summary>
    private static void AjustarInventario(
        List<UnidadeFranqueada> unidades,
        Dictionary<string, ProdutoServico> produtos,
        Dictionary<(int Unidade, int Produto), Estoque> estoques)
    {
        var savassi = unidades.Single(unidade => unidade.NomeFantasia == "Sabor Brasil Savassi");

        estoques[(savassi.Id, produtos["oleo"].Id)].Ajustar(6, "Inventário mensal: avaria em embalagens");
    }

    private static async Task CriarChamadosAsync(
        AppDbContext contexto,
        List<UnidadeFranqueada> unidades,
        Usuario gestor,
        Usuario operador,
        DateOnly hoje,
        CancellationToken cancellationToken)
    {
        PassoDeChamado[] roteiro =
        [
            new("Paulista", operador, "Sistema", "Terminal de vendas não imprime cupom", "Desde a manhã o terminal do caixa 2 não imprime o cupom da venda.", PrioridadeChamado.Critica, StatusChamado.EmAtendimento, 1, ["Impressora reiniciada, sem sucesso.", "Técnico agendado para hoje à tarde."]),
            new("Paulista", gestor, "Marketing", "Material da campanha de inverno", "Precisamos dos cartazes da campanha de inverno para a vitrine.", PrioridadeChamado.Baixa, StatusChamado.Aberto, 3, []),
            new("Campinas", operador, "Logística", "Entrega de suco atrasada", "O pedido de suco da semana não chegou e o estoque está acabando.", PrioridadeChamado.Alta, StatusChamado.Aberto, 2, ["Fornecedor informou atraso na rota."]),
            new("Campinas", gestor, "Financeiro", "Dúvida sobre cobrança de royalty", "Gostaria de entender o cálculo da última cobrança de royalty.", PrioridadeChamado.Media, StatusChamado.Resolvido, 12, ["Enviado o detalhamento do faturamento do mês.", "Cálculo conferido pela unidade."]),
            new("Savassi", gestor, "Manutenção", "Fritadeira com temperatura instável", "A fritadeira não mantém a temperatura e está atrasando os pedidos.", PrioridadeChamado.Alta, StatusChamado.EmAtendimento, 5, ["Peça de reposição solicitada ao fabricante."]),
            new("Savassi", operador, "Sistema", "Senha de acesso bloqueada", "O usuário do caixa foi bloqueado após tentativas de acesso.", PrioridadeChamado.Media, StatusChamado.Encerrado, 20, ["Senha redefinida pelo suporte.", "Acesso confirmado pela unidade."]),
            new("Curitiba", gestor, "Implantação", "Cronograma de inauguração", "Precisamos alinhar a data de inauguração e o treinamento da equipe.", PrioridadeChamado.Media, StatusChamado.Aberto, 4, ["Treinamento sugerido para a próxima semana."]),
            new("Paulista", gestor, "Logística", "Avaria em lote de refrigerantes", "Recebemos um lote com latas amassadas.", PrioridadeChamado.Baixa, StatusChamado.Encerrado, 30, ["Lote substituído pelo fornecedor."])
        ];

        var criados = new List<(ChamadoSuporte Chamado, int DiasAtras)>();

        foreach (var passo in roteiro)
        {
            var unidade = unidades.Single(registro => registro.NomeFantasia == $"Sabor Brasil {passo.Unidade}");
            var chamado = new ChamadoSuporte(unidade.Id, passo.Autor.Id, passo.Categoria, passo.Assunto, passo.Descricao, passo.Prioridade);

            foreach (var mensagem in passo.Mensagens)
            {
                chamado.RegistrarInteracao(AdministradorId, mensagem);
            }

            if (passo.Status != StatusChamado.Aberto)
            {
                chamado.AlterarStatus(passo.Status);
            }

            contexto.Chamados.Add(chamado);
            criados.Add((chamado, passo.DiasAtras));
        }

        await contexto.SaveChangesAsync(cancellationToken);

        // Os chamados foram abertos em dias diferentes, o que dá sentido ao filtro por período
        // e à fila de atendimento ordenada pela espera.
        foreach (var (chamado, diasAtras) in criados)
        {
            var abertura = hoje.AddDays(-diasAtras).ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc);

            await contexto.Chamados
                .Where(registro => registro.Id == chamado.Id)
                .ExecuteUpdateAsync(definicao => definicao.SetProperty(registro => registro.DataCriacao, abertura), cancellationToken);
        }
    }

    private static DateOnly PrimeiroDiaDoMes(DateOnly data) => new(data.Year, data.Month, 1);

    /// <summary>
    /// Um chamado do roteiro de demonstração: quem abriu, o relato, o estágio em que está,
    /// há quantos dias foi aberto e as mensagens trocadas com o suporte.
    /// </summary>
    private sealed record PassoDeChamado(
        string Unidade,
        Usuario Autor,
        string Categoria,
        string Assunto,
        string Descricao,
        PrioridadeChamado Prioridade,
        StatusChamado Status,
        int DiasAtras,
        string[] Mensagens);
}
