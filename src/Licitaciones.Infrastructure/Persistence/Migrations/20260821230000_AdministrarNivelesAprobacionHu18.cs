using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licitaciones.Infrastructure.Persistence.Migrations;

[DbContext(typeof(LicitacionesDbContext))]
[Migration("20260821230000_AdministrarNivelesAprobacionHu18")]
public partial class AdministrarNivelesAprobacionHu18 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "Activo",
            table: "NivelesAprobacion",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.Sql("""
            ALTER TABLE "NivelesAprobacion"
            DROP CONSTRAINT "EX_NivelesAprobacion_SinTraslape";

            ALTER TABLE "NivelesAprobacion"
            ADD CONSTRAINT "EX_NivelesAprobacion_SinTraslape"
            EXCLUDE USING gist (
                numrange("MontoMinimo", "MontoMaximo", '[)') WITH &&
            ) WHERE ("Activo");

            CREATE SEQUENCE "NivelesAprobacion_Id_seq" START WITH 4;
            ALTER SEQUENCE "NivelesAprobacion_Id_seq"
                OWNED BY "NivelesAprobacion"."Id";
            ALTER TABLE "NivelesAprobacion"
                ALTER COLUMN "Id" SET DEFAULT
                nextval('"NivelesAprobacion_Id_seq"'::regclass);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "NivelesAprobacion"
            ALTER COLUMN "Id" DROP DEFAULT;
            DROP SEQUENCE "NivelesAprobacion_Id_seq";

            ALTER TABLE "NivelesAprobacion"
            DROP CONSTRAINT "EX_NivelesAprobacion_SinTraslape";

            ALTER TABLE "NivelesAprobacion"
            ADD CONSTRAINT "EX_NivelesAprobacion_SinTraslape"
            EXCLUDE USING gist (
                numrange("MontoMinimo", "MontoMaximo", '[)') WITH &&
            );
            """);

        migrationBuilder.DropColumn(
            name: "Activo",
            table: "NivelesAprobacion");
    }
}
