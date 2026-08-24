using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Documentacion;

public sealed class DocumentacionDockerMarkdownTests
{
    [Fact]
    [Trait("HU", "HU-32")]
    public void DockerMd_DebeDocumentarInstruccionesReproduciblesDeUso()
    {
        var contenido = LeerDockerMd();

        foreach (var elemento in new[]
                 {
                     "docker compose up --build",
                     ".env",
                     "volumen",
                     "app",
                     "postgres"
                 })
        {
            Assert.True(
                contenido.Contains(elemento, StringComparison.OrdinalIgnoreCase),
                $"docs/docker.md debe documentar '{elemento}' como parte de las "
                + "instrucciones reproducibles del entorno completo.");
        }

        Assert.True(
            contenido.Contains("docker compose down", StringComparison.OrdinalIgnoreCase)
            || contenido.Contains("docker compose stop", StringComparison.OrdinalIgnoreCase),
            "docs/docker.md debe indicar cómo detener el entorno "
            + "('docker compose stop' o 'docker compose down').");
    }

    private static string LeerDockerMd()
    {
        var ruta = Path.Combine(
            RaizRepositorio.Obtener(),
            "docs",
            "docker.md");

        Assert.True(
            File.Exists(ruta),
            $"Debe existir la documentación de Docker en {ruta}.");

        return File.ReadAllText(ruta);
    }
}
