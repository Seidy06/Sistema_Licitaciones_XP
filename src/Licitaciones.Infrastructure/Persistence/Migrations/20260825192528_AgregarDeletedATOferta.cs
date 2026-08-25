using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licitaciones.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDeletedATOferta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ofertas_LicitacionId_ProveedorId",
                table: "Ofertas");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Ofertas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_LicitacionId_ProveedorId",
                table: "Ofertas",
                columns: new[] { "LicitacionId", "ProveedorId" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ofertas_LicitacionId_ProveedorId",
                table: "Ofertas");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Ofertas");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_LicitacionId_ProveedorId",
                table: "Ofertas",
                columns: new[] { "LicitacionId", "ProveedorId" },
                unique: true);
        }
    }
}
