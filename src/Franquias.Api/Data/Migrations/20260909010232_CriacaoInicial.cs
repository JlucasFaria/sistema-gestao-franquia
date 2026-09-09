using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Franquias.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RazaoSocial = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    NomeFantasia = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Uf = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 2, nullable: false),
                    Cep = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 8, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Franqueadoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RazaoSocial = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    NomeFantasia = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Uf = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 2, nullable: false),
                    Cep = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 8, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Franqueadoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Franqueados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    DataAdesao = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Franqueados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PrecoBase = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    EhServico = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Unidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FranqueadoraId = table.Column<int>(type: "INTEGER", nullable: false),
                    FranqueadoId = table.Column<int>(type: "INTEGER", nullable: false),
                    RazaoSocial = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    NomeFantasia = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Uf = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 2, nullable: false),
                    Cep = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 8, nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Situacao = table.Column<int>(type: "INTEGER", nullable: false),
                    PercentualRoyalty = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unidades_Franqueadoras_FranqueadoraId",
                        column: x => x.FranqueadoraId,
                        principalTable: "Franqueadoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Unidades_Franqueados_FranqueadoId",
                        column: x => x.FranqueadoId,
                        principalTable: "Franqueados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PerfilId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Perfis_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FornecedoresProdutos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FornecedorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdutoServicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoFornecimento = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PrazoEntregaEmDias = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedoresProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FornecedoresProdutos_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FornecedoresProdutos_Produtos_ProdutoServicoId",
                        column: x => x.ProdutoServicoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estoques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnidadeFranqueadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdutoServicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estoques_Produtos_ProdutoServicoId",
                        column: x => x.ProdutoServicoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estoques_Unidades_UnidadeFranqueadaId",
                        column: x => x.UnidadeFranqueadaId,
                        principalTable: "Unidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Responsaveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 11, nullable: false),
                    Cargo = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    UnidadeFranqueadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsaveis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responsaveis_Unidades_UnidadeFranqueadaId",
                        column: x => x.UnidadeFranqueadaId,
                        principalTable: "Unidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Royalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnidadeFranqueadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PeriodoInicio = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PeriodoFim = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    FaturamentoBase = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PercentualAplicado = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    ValorDevido = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DataVencimento = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Situacao = table.Column<int>(type: "INTEGER", nullable: false),
                    DataPagamento = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ValorPago = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Royalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Royalties_Unidades_UnidadeFranqueadaId",
                        column: x => x.UnidadeFranqueadaId,
                        principalTable: "Unidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnidadeFranqueadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataVenda = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Unidades_UnidadeFranqueadaId",
                        column: x => x.UnidadeFranqueadaId,
                        principalTable: "Unidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chamados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnidadeFranqueadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioAberturaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Assunto = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Prioridade = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DataEncerramento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chamados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chamados_Unidades_UnidadeFranqueadaId",
                        column: x => x.UnidadeFranqueadaId,
                        principalTable: "Unidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chamados_Usuarios_UsuarioAberturaId",
                        column: x => x.UsuarioAberturaId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EstoqueId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    SaldoAnterior = table.Column<int>(type: "INTEGER", nullable: false),
                    SaldoResultante = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Estoques_EstoqueId",
                        column: x => x.EstoqueId,
                        principalTable: "Estoques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdutoServicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Produtos_ProdutoServicoId",
                        column: x => x.ProdutoServicoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteracoesChamado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChamadoSuporteId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteracoesChamado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteracoesChamado_Chamados_ChamadoSuporteId",
                        column: x => x.ChamadoSuporteId,
                        principalTable: "Chamados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InteracoesChamado_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nome",
                table: "Categorias",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_UnidadeFranqueadaId_Status_Prioridade",
                table: "Chamados",
                columns: new[] { "UnidadeFranqueadaId", "Status", "Prioridade" });

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_UsuarioAberturaId",
                table: "Chamados",
                column: "UsuarioAberturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_ProdutoServicoId",
                table: "Estoques",
                column: "ProdutoServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_UnidadeFranqueadaId_ProdutoServicoId",
                table: "Estoques",
                columns: new[] { "UnidadeFranqueadaId", "ProdutoServicoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_Cnpj",
                table: "Fornecedores",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FornecedoresProdutos_FornecedorId_ProdutoServicoId",
                table: "FornecedoresProdutos",
                columns: new[] { "FornecedorId", "ProdutoServicoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FornecedoresProdutos_ProdutoServicoId",
                table: "FornecedoresProdutos",
                column: "ProdutoServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Franqueadoras_Cnpj",
                table: "Franqueadoras",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Franqueados_Cpf",
                table: "Franqueados",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InteracoesChamado_ChamadoSuporteId",
                table: "InteracoesChamado",
                column: "ChamadoSuporteId");

            migrationBuilder.CreateIndex(
                name: "IX_InteracoesChamado_UsuarioId",
                table: "InteracoesChamado",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_ProdutoServicoId",
                table: "ItensVenda",
                column: "ProdutoServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_VendaId",
                table: "ItensVenda",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_EstoqueId_DataCriacao",
                table: "MovimentacoesEstoque",
                columns: new[] { "EstoqueId", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_Perfis_Codigo",
                table: "Perfis",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Nome",
                table: "Produtos",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_UnidadeFranqueadaId",
                table: "Responsaveis",
                column: "UnidadeFranqueadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Royalties_UnidadeFranqueadaId_PeriodoInicio_PeriodoFim",
                table: "Royalties",
                columns: new[] { "UnidadeFranqueadaId", "PeriodoInicio", "PeriodoFim" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Royalties_UnidadeFranqueadaId_Situacao",
                table: "Royalties",
                columns: new[] { "UnidadeFranqueadaId", "Situacao" });

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_Cnpj",
                table: "Unidades",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_FranqueadoId",
                table: "Unidades",
                column: "FranqueadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_FranqueadoraId",
                table: "Unidades",
                column: "FranqueadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PerfilId",
                table: "Usuarios",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_UnidadeFranqueadaId_DataVenda",
                table: "Vendas",
                columns: new[] { "UnidadeFranqueadaId", "DataVenda" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FornecedoresProdutos");

            migrationBuilder.DropTable(
                name: "InteracoesChamado");

            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "Responsaveis");

            migrationBuilder.DropTable(
                name: "Royalties");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "Chamados");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Estoques");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Unidades");

            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Franqueadoras");

            migrationBuilder.DropTable(
                name: "Franqueados");
        }
    }
}
