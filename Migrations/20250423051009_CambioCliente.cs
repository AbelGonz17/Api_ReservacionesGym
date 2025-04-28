using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiReservacionesGym.Migrations
{
    /// <inheritdoc />
    public partial class CambioCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Membresias_MembresiaId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_MembresiaId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "MembresiaId",
                table: "Clientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembresiaId",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_MembresiaId",
                table: "Clientes",
                column: "MembresiaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Membresias_MembresiaId",
                table: "Clientes",
                column: "MembresiaId",
                principalTable: "Membresias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
