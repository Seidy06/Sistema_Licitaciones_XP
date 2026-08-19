using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licitaciones.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImplementPublishTenderHu11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "licitacion_transiciones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    licitacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estado_anterior = table.Column<int>(type: "integer", nullable: false),
                    estado_nuevo = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_licitacion_transiciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_licitacion_transiciones_Licitaciones_licitacion_id",
                        column: x => x.licitacion_id,
                        principalTable: "Licitaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_licitacion_transiciones_licitacion_id",
                table: "licitacion_transiciones",
                column: "licitacion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "licitacion_transiciones");
        }
    }
}
