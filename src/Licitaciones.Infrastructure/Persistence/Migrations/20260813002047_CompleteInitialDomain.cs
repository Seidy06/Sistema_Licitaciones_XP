using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Licitaciones.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteInitialDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosLicitacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosLicitacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NivelesAprobacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MontoMinimo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MontoMaximo = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesAprobacion", x => x.Id);
                    table.CheckConstraint("CK_NivelesAprobacion_Minimo", "\"MontoMinimo\" >= 0");
                    table.CheckConstraint("CK_NivelesAprobacion_Rango", "\"MontoMaximo\" IS NULL OR \"MontoMaximo\" > \"MontoMinimo\"");
                });

            migrationBuilder.CreateTable(
                name: "TiposCambio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    MonedaOrigen = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    MonedaDestino = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCambio", x => x.Id);
                    table.CheckConstraint("CK_TiposCambio_Valor_Positivo", "\"Valor\" > 0");
                });

            migrationBuilder.Sql("""
                ALTER TABLE "NivelesAprobacion"
                ADD CONSTRAINT "EX_NivelesAprobacion_SinTraslape"
                EXCLUDE USING gist (
                    numrange("MontoMinimo", "MontoMaximo", '[)') WITH &&
                );
                """);

            migrationBuilder.CreateTable(
                name: "Licitaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Presupuesto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaCierre = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licitaciones", x => x.Id);
                    table.CheckConstraint("CK_Licitaciones_Presupuesto_Positivo", "\"Presupuesto\" > 0");
                    table.ForeignKey(
                        name: "FK_Licitaciones_EstadosLicitacion_Estado",
                        column: x => x.Estado,
                        principalTable: "EstadosLicitacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LicitacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaRegistro = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.Id);
                    table.CheckConstraint("CK_Ofertas_Monto_Positivo", "\"Monto\" > 0");
                    table.ForeignKey(
                        name: "FK_Ofertas_Licitaciones_LicitacionId",
                        column: x => x.LicitacionId,
                        principalTable: "Licitaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ofertas_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EstadosLicitacion",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Borrador" },
                    { 2, "Publicada" },
                    { 3, "Cerrada" },
                    { 4, "Adjudicada" },
                    { 5, "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "NivelesAprobacion",
                columns: new[] { "Id", "CreatedAt", "MontoMaximo", "MontoMinimo", "Nombre", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1000000m, 0m, "Operativo", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10000000m, 1000000m, "Gerencial", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 10000000m, "Directivo", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "TiposCambio",
                columns: new[] { "Id", "Activo", "CreatedAt", "Fecha", "MonedaDestino", "MonedaOrigen", "UpdatedAt", "Valor" },
                values: new object[] { 1, true, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateOnly(2026, 1, 1), "CRC", "USD", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 500m });

            migrationBuilder.CreateIndex(
                name: "IX_EstadosLicitacion_Nombre",
                table: "EstadosLicitacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Licitaciones_Codigo",
                table: "Licitaciones",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Licitaciones_Estado",
                table: "Licitaciones",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_LicitacionId_ProveedorId",
                table: "Ofertas",
                columns: new[] { "LicitacionId", "ProveedorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_ProveedorId",
                table: "Ofertas",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "UX_TiposCambio_Activo",
                table: "TiposCambio",
                column: "Activo",
                unique: true,
                filter: "\"Activo\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NivelesAprobacion");

            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.DropTable(
                name: "TiposCambio");

            migrationBuilder.DropTable(
                name: "Licitaciones");

            migrationBuilder.DropTable(
                name: "EstadosLicitacion");
        }
    }
}
