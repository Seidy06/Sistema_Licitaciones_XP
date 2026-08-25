using Licitaciones.Api.Contracts.Aprobaciones;
using Licitaciones.Api.Contracts.Licitaciones;
using Licitaciones.Api.Contracts.Ofertas;
using Licitaciones.Api.Contracts.Proveedores;
using Licitaciones.Api.Contracts.TiposCambio;

using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Licitaciones;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Licitaciones.Api.Infraestructura;

/// <summary>
/// Filtro de esquema Swagger que agrega ejemplos a los tipos de DTO y request de la API.
/// </summary>
public sealed class EjemplosEsquemasFiltro : ISchemaFilter
{
    private static readonly Dictionary<Type, Func<OpenApiObject>> EjemplosPorTipo = new()
    {
        [typeof(ProveedorDto)] = EjemploProveedor,
        [typeof(LicitacionDto)] = EjemploLicitacion,
        [typeof(OfertaDto)] = EjemploOferta,
        [typeof(TipoCambioDto)] = EjemploTipoCambio,
        [typeof(CrearProveedorRequest)] = EjemploCrearProveedor,
        [typeof(EditarProveedorRequest)] = EjemploEditarProveedor,
        [typeof(CrearLicitacionRequest)] = EjemploCrearLicitacion,
        [typeof(EditarLicitacionRequest)] = EjemploEditarLicitacion,
        [typeof(CrearOfertaRequest)] = EjemploCrearOferta,
        [typeof(GuardarNivelAprobacionRequest)] = EjemploGuardarNivelAprobacion,
        [typeof(GuardarTipoCambioRequest)] = EjemploGuardarTipoCambio
    };

    /// <summary>
    /// Aplica el ejemplo correspondiente al esquema Swagger si el tipo tiene un ejemplo registrado.
    /// </summary>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        if (context.Type is { } tipo && EjemplosPorTipo.TryGetValue(tipo, out var ejemplo))
        {
            schema.Example = ejemplo();
        }
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

    private static OpenApiObject EjemploCrearProveedor() => new()
    {
        ["nombre"] = new OpenApiString("Empresa Central S.A.")
    };

    private static OpenApiObject EjemploEditarProveedor() => new()
    {
        ["nombre"] = new OpenApiString("Empresa Central S.A. Actualizada"),
        ["version"] = new OpenApiInteger(1)
    };

    private static OpenApiObject EjemploCrearLicitacion() => new()
    {
        ["codigo"] = new OpenApiString("COMP-2026-001"),
        ["titulo"] = new OpenApiString("Compra de material informático"),
        ["presupuesto"] = new OpenApiDouble(10000.00),
        ["fechaCierre"] = new OpenApiString("2026-08-25T12:00:00+00:00")
    };

    private static OpenApiObject EjemploEditarLicitacion() => new()
    {
        ["titulo"] = new OpenApiString("Compra de material informático renovada"),
        ["presupuesto"] = new OpenApiDouble(12000.00),
        ["fechaCierre"] = new OpenApiString("2026-09-01T12:00:00+00:00")
    };

    private static OpenApiObject EjemploCrearOferta() => new()
    {
        ["licitacionId"] = new OpenApiString("d5d2f6a1-3c47-4a58-9b21-0f6e8d2c4b10"),
        ["proveedorId"] = new OpenApiString("7d9413f2-2bde-4bc9-af45-39a66f8fcce5"),
        ["monto"] = new OpenApiDouble(8000.00)
    };

    private static OpenApiObject EjemploGuardarNivelAprobacion() => new()
    {
        ["nombre"] = new OpenApiString("Compras Menores"),
        ["montoMinimo"] = new OpenApiDouble(0),
        ["montoMaximo"] = new OpenApiDouble(1000000)
    };

    private static OpenApiObject EjemploGuardarTipoCambio() => new()
    {
        ["valor"] = new OpenApiDouble(512.00),
        ["fecha"] = new OpenApiString("2026-08-22")
    };
}
