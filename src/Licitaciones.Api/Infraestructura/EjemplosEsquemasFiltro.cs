using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Licitaciones;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Licitaciones.Api.Infraestructura;

public sealed class EjemplosEsquemasFiltro : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        schema.Example = context.Type == typeof(ProveedorDto) ? EjemploProveedor()
            : context.Type == typeof(LicitacionDto) ? EjemploLicitacion()
            : context.Type == typeof(OfertaDto) ? EjemploOferta()
            : context.Type == typeof(TipoCambioDto) ? EjemploTipoCambio()
            : null;
    }

    private static OpenApiObject EjemploProveedor() => new()
    {
        ["id"] = new OpenApiString("7d9413f2-2bde-4bc9-af45-39a66f8fcce5"),
        ["nombre"] = new OpenApiString("Empresa Central S.A."),
        ["nombreNormalizado"] = new OpenApiString("EMPRESA CENTRAL S.A."),
        ["createdAt"] = new OpenApiString("2026-08-15T12:00:00+00:00"),
        ["updatedAt"] = new OpenApiString("2026-08-15T12:00:00+00:00"),
        ["version"] = new OpenApiInteger(1)
    };

    private static OpenApiObject EjemploLicitacion() => new()
    {
        ["id"] = new OpenApiString("d5d2f6a1-3c47-4a58-9b21-0f6e8d2c4b10"),
        ["codigo"] = new OpenApiString("COMP-2026-001"),
        ["codigoNormalizado"] = new OpenApiString("COMP-2026-001"),
        ["titulo"] = new OpenApiString("Compra de material informático"),
        ["presupuesto"] = new OpenApiDouble(10000.00),
        ["fechaCierre"] = new OpenApiString("2026-08-25T12:00:00+00:00"),
        ["estado"] = new OpenApiString(nameof(EstadoLicitacion.Publicada)),
        ["createdAt"] = new OpenApiString("2026-08-01T08:30:00+00:00"),
        ["updatedAt"] = new OpenApiString("2026-08-02T09:15:00+00:00")
    };

    private static OpenApiObject EjemploOferta() => new()
    {
        ["id"] = new OpenApiString("9a3d94d0-5e61-4f7b-8c22-1a2b3c4d5e6f"),
        ["licitacionId"] = new OpenApiString("d5d2f6a1-3c47-4a58-9b21-0f6e8d2c4b10"),
        ["proveedorId"] = new OpenApiString("7d9413f2-2bde-4bc9-af45-39a66f8fcce5"),
        ["monto"] = new OpenApiDouble(8000.00),
        ["fechaRegistro"] = new OpenApiString("2026-08-19T15:00:00+00:00")
    };

    private static OpenApiObject EjemploTipoCambio() => new()
    {
        ["id"] = new OpenApiInteger(2),
        ["monedaOrigen"] = new OpenApiString("USD"),
        ["monedaDestino"] = new OpenApiString("CRC"),
        ["valor"] = new OpenApiDouble(512.00),
        ["fecha"] = new OpenApiString("2026-08-22"),
        ["activo"] = new OpenApiBoolean(true)
    };
}
