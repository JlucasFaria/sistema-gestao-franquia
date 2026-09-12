using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Franquias.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestricaoSaldoNaoNegativo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Estoques_QuantidadeMinimaNaoNegativa",
                table: "Estoques",
                sql: "\"QuantidadeMinima\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Estoques_QuantidadeNaoNegativa",
                table: "Estoques",
                sql: "\"Quantidade\" >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Estoques_QuantidadeMinimaNaoNegativa",
                table: "Estoques");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Estoques_QuantidadeNaoNegativa",
                table: "Estoques");
        }
    }
}
