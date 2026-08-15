using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licitaciones.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProveedorSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Proveedores_NombreNormalizado",
                table: "Proveedores");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Proveedores",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_Proveedores_NombreNormalizado",
                table: "Proveedores",
                column: "NombreNormalizado",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Proveedores_NombreNormalizado",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Proveedores");

            migrationBuilder.CreateIndex(
                name: "UX_Proveedores_NombreNormalizado",
                table: "Proveedores",
                column: "NombreNormalizado",
                unique: true);
        }
    }
}
