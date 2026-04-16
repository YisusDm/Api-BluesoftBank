using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BluesoftBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexCedulaNit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cedula",
                table: "Clientes",
                column: "Cedula",
                unique: true,
                filter: "[Cedula] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Nit",
                table: "Clientes",
                column: "Nit",
                unique: true,
                filter: "[Nit] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_Cedula",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Nit",
                table: "Clientes");
        }
    }
}
