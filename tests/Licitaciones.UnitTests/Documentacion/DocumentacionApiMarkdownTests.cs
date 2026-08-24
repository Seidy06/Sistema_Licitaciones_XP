using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Documentacion;

public sealed class DocumentacionApiMarkdownTests
{
    private static readonly string[] RecursosApi =
    {
        "api/v1/proveedores",
        "api/v1/licitaciones",
        "api/v1/ofertas",
        "api/v1/niveles-aprobacion",
        "api/v1/tipos-cambio"
    };

    [Fact]
    [Trait("HU", "HU-27")]
    public void ApiMd_DebeDocumentarEndpointsContratosErroresYEjemplos()
    {
        var contenido = LeerDocumentacionApi();

        foreach (var recurso in RecursosApi)
        {
            Assert.Contains(
                recurso,
                contenido,
                StringComparison.OrdinalIgnoreCase);
        }

        foreach (var contrato in new[]
                 {
                     "ProblemDetails",
                     "codigoError",
                     "correlacionId"
                 })
        {
            Assert.Contains(contrato, contenido, StringComparison.Ordinal);
        }

        foreach (var codigo in new[] { "400", "404", "409", "422" })
        {
            Assert.Contains(codigo, contenido, StringComparison.Ordinal);
        }

        Assert.True(
            ContarBloques(contenido, "```json") >= 5,
            "La documentación debe incluir ejemplos de respuesta en bloques json.");
        Assert.True(
            ContarBloques(contenido, "```http") >= 3,
            "La documentación debe incluir ejemplos de solicitudes en bloques http.");
    }

    [Fact]
    [Trait("HU", "HU-27")]
    public void ApiMd_DebeReferenciarColeccionReproducibleExistenteYCubrirRecursos()
    {
        var contenido = LeerDocumentacionApi();
        var raiz = RaizRepositorio.Obtener();

        var coincidencias = Regex.Matches(
            contenido,
            @"[A-Za-z0-9._\-]+\.(?:http|postman_collection\.json)",
            RegexOptions.IgnoreCase);

        var candidatas = coincidencias
            .Select(coincidencia => coincidencia.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.True(
            candidatas.Length > 0,
            "docs/api.md debe referenciar una colección reproducible de solicitudes " +
            "(archivo .http o colección Postman).");

        var rutaColeccion = candidatas
            .Select(nombre => File.Exists(Path.Combine(raiz, "docs", nombre))
                ? Path.Combine(raiz, "docs", nombre)
                : Path.Combine(raiz, nombre))
            .FirstOrDefault(File.Exists);

        Assert.True(
            rutaColeccion is not null,
            $"La colección referenciada ({string.Join(", ", candidatas)}) debe existir en el repositorio.");

        var contenidoColeccion = File.ReadAllText(rutaColeccion);

        foreach (var recurso in RecursosApi)
        {
            Assert.True(
                contenidoColeccion.Contains(recurso, StringComparison.OrdinalIgnoreCase),
                $"La colección '{Path.GetFileName(rutaColeccion)}' debe incluir solicitudes para '{recurso}'.");
        }
    }

    private static string LeerDocumentacionApi()
    {
        var ruta = Path.Combine(
            RaizRepositorio.Obtener(),
            "docs",
            "api.md");

        Assert.True(
            File.Exists(ruta),
            $"Debe existir la documentación de la API en {ruta}.");

        return File.ReadAllText(ruta);
    }

    private static int ContarBloques(string contenido, string cerca)
    {
        var total = 0;
        var indice = contenido.IndexOf(cerca, StringComparison.Ordinal);

        while (indice >= 0)
        {
            total++;
            indice = contenido.IndexOf(cerca, indice + cerca.Length, StringComparison.Ordinal);
        }

        return total;
    }
}
