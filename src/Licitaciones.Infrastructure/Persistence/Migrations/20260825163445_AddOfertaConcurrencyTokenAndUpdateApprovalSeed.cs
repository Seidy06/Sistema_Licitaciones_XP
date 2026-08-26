using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licitaciones.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOfertaConcurrencyTokenAndUpdateApprovalSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Ofertas",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.UpdateData(
                table: "NivelesAprobacion",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MontoMaximo", "MontoMinimo", "Nombre" },
                values: new object[] { 999999.99m, 0.01m, "Encargado de area" });

            migrationBuilder.UpdateData(
                table: "NivelesAprobacion",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MontoMaximo", "Nombre" },
                values: new object[] { 9999999.99m, "Gerencia" });

            migrationBuilder.UpdateData(
                table: "NivelesAprobacion",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Junta Directiva");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Ofertas");

            migrationBuilder.UpdateData(
                table: "NivelesAprobacion",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MontoMaximo", "MontoMinimo", "Nombre" },
                values: new object[] { 1000000m, 0m, "Operativo" });

            migrationBuilder.UpdateData(
                table: "NivelesAprobacion",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MontoMaximo", "Nombre" },
                values: new object[] { 10000000m, "Gerencial" });

            migrationBuilder.UpdateData(
                table: "NivelesAprobacion",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Directivo");
        }
    }
}
