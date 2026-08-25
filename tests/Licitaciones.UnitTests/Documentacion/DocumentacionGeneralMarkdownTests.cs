using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Documentacion;

public sealed class DocumentacionGeneralMarkdownTests
{
    private static readonly string[] DocumentosConDiagrama =
        { "arquitectura-general.md", "modelo-datos.md" };

    [Fact]
    [Trait("HU", "HU-36")]
    public void Readme_DebeFuncionarComoIndiceDeNavegacionDeTodaLaDocumentacion()
    {
        var raizDocs = RaizDocs();
        var readme = LeerDocumento("README.md");

        var documentos = Directory
            .EnumerateFiles(raizDocs, "*.md", SearchOption.AllDirectories)
            .Select(ruta => Path.GetRelativePath(raizDocs, ruta).Replace('\\', '/'))
            .Where(ruta => !ruta.Equals("README.md", StringComparison.OrdinalIgnoreCase))
            .OrderBy(ruta => ruta, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Assert.NotEmpty(documentos);

        foreach (var documento in documentos)
        {
            Assert.True(
                Regex.IsMatch(
                    readme,
                    @"\]\(\s*" + Regex.Escape(documento) + @"\s*\)",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant),
                $"'docs/README.md' debe enlazar 'docs/{documento}' para funcionar "
                + "como índice de navegación de toda la documentación.");
        }

        foreach (Match enlace in Regex.Matches(
                     readme,
                     @"\]\(([^)#\s]+\.md)",
                     RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            var destino = enlace.Groups[1].Value.Replace('/', Path.DirectorySeparatorChar);

            Assert.True(
                File.Exists(Path.Combine(raizDocs, destino)),
                $"'docs/README.md' enlaza '{enlace.Groups[1].Value}' pero el "
                + "archivo no existe; el índice de navegación no puede tener "
                + "enlaces rotos.");
        }
    }

    [Fact]
    [Trait("HU", "HU-36")]
    public void ArquitecturaGeneralYModeloDeDatos_DebenIncluirDiagramasMermaidOImagenesDeAssets()
    {
        foreach (var documento in DocumentosConDiagrama)
        {
            var contenido = LeerDocumento(documento);

            var tieneMermaid = Regex.IsMatch(
                contenido,
                @"```mermaid\s+\S[\s\S]*?```",
                RegexOptions.CultureInvariant);

            var imagenValida = TieneImagenDeAssets(contenido, documento);

            Assert.True(
                tieneMermaid || imagenValida,
                $"'docs/{documento}' debe incluir un diagrama Mermaid o una "
                + "imagen existente en 'docs/assets' según el criterio de HU-36.");
        }
    }

    [Fact]
    [Trait("HU", "HU-36")]
    public void Bitacora_DebeRegistrarPorIteracionResultadosVelocidadRetroalimentacionCiclosRefactorYLiberaciones()
    {
        var contenido = LeerDocumento("bitacora-xp.md");

        var bloques = Regex.Split(
            contenido,
            @"(?=^## Iteración\s+\d)",
            RegexOptions.Multiline | RegexOptions.CultureInvariant);

        foreach (var numeroIteracion in new[] { 1, 2, 3, 4 })
        {
            var bloque = bloques.SingleOrDefault(texto =>
                Regex.IsMatch(
                    texto,
                    @"^## Iteración\s+" + numeroIteracion + @"\b",
                    RegexOptions.Multiline | RegexOptions.CultureInvariant));

            Assert.True(
                bloque is not null,
                $"'docs/bitacora-xp.md' debe registrar la iteración "
                + $"{numeroIteracion} con su propia sección.");

            RequerirMarcador(bloque!, "resultado", numeroIteracion);
            RequerirMarcador(bloque!, "velocidad", numeroIteracion);
            RequerirMarcador(bloque!, "retroalimentaci|ajustes", numeroIteracion);
            RequerirMarcador(bloque!, "rojo", numeroIteracion);
            RequerirMarcador(bloque!, "verde", numeroIteracion);
            RequerirMarcador(bloque!, "refactor", numeroIteracion);
            RequerirMarcador(bloque!, "liberaci|etiqueta v|tag v", numeroIteracion);
        }
    }

    [Fact]
    [Trait("HU", "HU-36")]
    public void UsoIa_DebeDeclararHerramientaFinalidadModulosEjemplosYValidacionesDelEquipo()
    {
        var contenido = LeerDocumento("uso-ia.md");

        var declaracionAlcance = Regex.Match(
            contenido,
            @"##\s+.*(alcance|finalidad)[\s\S]*?(?=\n##\s)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        Assert.True(
            declaracionAlcance.Success,
            "'docs/uso-ia.md' debe declarar el alcance y la finalidad del uso "
            + "de IA en una sección propia.");

        Assert.True(
            Regex.IsMatch(
                declaracionAlcance.Value,
                @"(herramienta|utilizad[oa])[^.\r\n]*(codex|claude|copilot|chatgpt|opencode)"
                + "|(codex|claude|copilot|chatgpt|opencode)[^.\r\n]*(herramienta|utilizad[oa])",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant),
            "La declaración de alcance de 'docs/uso-ia.md' debe nombrar la "
            + "herramienta de IA utilizada de forma explícita; indicar solo "
            + "'IA' de manera genérica no declara qué herramienta se usó.");

        foreach (var elemento in new[] { "Módulos asistidos", "Ejemplos", "validaci" })
        {
            Assert.True(
                contenido.Contains(elemento, StringComparison.OrdinalIgnoreCase),
                $"'docs/uso-ia.md' debe declarar '{elemento}' según el criterio "
                + "de uso responsable de herramientas de IA de HU-36.");
        }
    }

    private static bool TieneImagenDeAssets(string contenido, string documento)
    {
        var imagen = Regex.Match(
            contenido,
            @"!\[[^\]]*\]\(([^)]+\.(?:png|svg|jpg|jpeg))\)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        if (!imagen.Success)
        {
            return false;
        }

        var rutaDocumento = Path.Combine(RaizDocs(), documento);
        var rutaImagen = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(rutaDocumento)!,
            imagen.Groups[1].Value.Replace('/', Path.DirectorySeparatorChar)));

        return File.Exists(rutaImagen);
    }

    private static void RequerirMarcador(string bloque, string patron, int numeroIteracion)
    {
        var etiqueta = patron.Replace("|", "' o '");

        Assert.True(
            Regex.IsMatch(
                bloque,
                patron,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant),
            $"La sección de la iteración {numeroIteracion} de "
            + "'docs/bitacora-xp.md' debe registrar '" + etiqueta + "' "
            + "como parte de los resultados, velocidad, retroalimentación, "
            + "ciclos TDD, refactorizaciones y pequeñas liberaciones exigidos "
            + "por iteración en HU-36.");
    }

    private static string RaizDocs()
    {
        return Path.Combine(RaizRepositorio.Obtener(), "docs");
    }

    private static string LeerDocumento(string nombreArchivo)
    {
        var ruta = Path.Combine(RaizDocs(), nombreArchivo);

        Assert.True(
            File.Exists(ruta),
            $"Debe existir la documentación '{nombreArchivo}' dentro de 'docs/' "
            + "para cumplir la documentación mínima requerida por HU-36.");

        return File.ReadAllText(ruta);
    }
}
