using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Franquias.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class DadosIniciais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Ativo", "DataAtualizacao", "DataCriacao", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Itens alimentícios preparados ou industrializados.", "Alimentos" },
                    { 2, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bebidas quentes, geladas e alcoólicas.", "Bebidas" },
                    { 3, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Matérias-primas e embalagens usadas na operação da unidade.", "Insumos" },
                    { 4, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Serviços prestados pelas unidades aos clientes da rede.", "Serviços" }
                });

            migrationBuilder.InsertData(
                table: "Franqueadoras",
                columns: new[] { "Id", "Ativo", "Cnpj", "DataAtualizacao", "DataCriacao", "Email", "NomeFantasia", "RazaoSocial", "Telefone", "Bairro", "Cep", "Cidade", "Complemento", "Logradouro", "Numero", "Uf" },
                values: new object[] { 1, true, "11222333000181", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "contato@saborbrasil.com.br", "Sabor Brasil", "Rede Sabor Brasil Franquias LTDA", "1133334444", "Bela Vista", "01310100", "Sao Paulo", "10 andar", "Avenida Paulista", "1000", "SP" });

            migrationBuilder.InsertData(
                table: "Perfis",
                columns: new[] { "Id", "Ativo", "Codigo", "DataAtualizacao", "DataCriacao", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso total à rede, incluindo cadastros da franqueadora e relatórios consolidados.", "Administrador" },
                    { 2, true, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gerencia estoque, vendas e chamados da própria unidade franqueada.", "Gestor de Unidade" },
                    { 3, true, 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso operacional ao registro de vendas e às movimentações de estoque.", "Operador" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "DataAtualizacao", "DataCriacao", "Email", "Nome", "PerfilId", "SenhaHash" },
                values: new object[] { 1, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@franquias.com.br", "Administrador do Sistema", 1, "$2a$11$b26efR43sVZtVMymT7DMvuqI4FvaLSEtSV0QfROWfIENmdoXSRlEW" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Franqueadoras",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Perfis",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Perfis",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Perfis",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
