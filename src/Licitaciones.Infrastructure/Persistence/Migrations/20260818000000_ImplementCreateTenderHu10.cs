using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licitaciones.Infrastructure.Persistence.Migrations;

[DbContext(typeof(LicitacionesDbContext))]
[Migration("20260818000000_ImplementCreateTenderHu10")]
public sealed class ImplementCreateTenderHu10 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Licitaciones_Codigo",
            table: "Licitaciones");

        migrationBuilder.AddColumn<string>(
            name: "CodigoNormalizado",
            table: "Licitaciones",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DeletedAt",
            table: "Licitaciones",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.Sql(
            "UPDATE \"Licitaciones\" " +
            "SET \"CodigoNormalizado\" = upper(trim(\"Codigo\"));");

        migrationBuilder.CreateIndex(
            name: "UX_Licitaciones_CodigoNormalizado",
            table: "Licitaciones",
            column: "CodigoNormalizado",
            unique: true,
            filter: "\"DeletedAt\" IS NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UX_Licitaciones_CodigoNormalizado",
            table: "Licitaciones");

        migrationBuilder.DropColumn(
            name: "CodigoNormalizado",
            table: "Licitaciones");

        migrationBuilder.DropColumn(
            name: "DeletedAt",
            table: "Licitaciones");

        migrationBuilder.CreateIndex(
            name: "IX_Licitaciones_Codigo",
            table: "Licitaciones",
            column: "Codigo",
            unique: true);
    }
}
