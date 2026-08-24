using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Despliegue;

public sealed class DockerfileTests
{
    [Fact]
    [Trait("HU", "HU-31")]
    public void Dockerfile_DebeExistirEnLaRaizDelRepositorio() => _ = LeerDockerfile();

    [Fact]
    [Trait("HU", "HU-31")]
    public void Dockerfile_DebeUsarEtapaBuildConImagenSdk9()
    {
        var etapas = ExtraerEtapas(LeerDockerfile());

        var build = etapas
            .Cast<(string Imagen, string Nombre)?>()
            .FirstOrDefault(etapa =>
                etapa.Value.Nombre.Equals("build", StringComparison.OrdinalIgnoreCase));

        Assert.True(
            build is not null,
            "El Dockerfile debe declarar una etapa con la instrucción 'AS build'.");

        Assert.True(
            build.Value.Imagen.EndsWith(
                "/dotnet/sdk:9.0",
                StringComparison.OrdinalIgnoreCase),
            $"La etapa 'build' debe partir de la imagen SDK .NET 9 " +
            $"('{ImagenSdk}'); se encontró '{build.Value.Imagen}'.");
    }

    [Fact]
    [Trait("HU", "HU-31")]
    public void Dockerfile_DebeSerMultiStageConEtapaFinalRuntimeAspnet9()
    {
        var etapas = ExtraerEtapas(LeerDockerfile());

        Assert.True(
            etapas.Length >= 2,
            "El Dockerfile debe ser multi-stage: una etapa 'build' con el SDK "
            + "y una etapa final de ejecución.");

        var final = etapas[^1];

        Assert.True(
            final.Imagen.EndsWith(
                "/dotnet/aspnet:9.0",
                StringComparison.OrdinalIgnoreCase),
            $"La etapa final debe usar únicamente el runtime ASP.NET .NET 9 " +
            $"('{ImagenRuntime}'); se encontró '{final.Imagen}'.");
    }

    [Fact]
    [Trait("HU", "HU-31")]
    public void Dockerfile_EtapaFinalDebeEjecutarConUsuarioNoRoot()
    {
        var seccionFinal = ObtenerSeccionEtapaFinal(LeerDockerfile());

        var usuario = Regex.Match(
            seccionFinal,
            @"^USER\s+(\S+)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        Assert.True(
            usuario.Success,
            "La etapa final del Dockerfile debe declarar 'USER' con un usuario no root.");

        Assert.False(
            usuario.Groups[1].Value.Equals("root", StringComparison.OrdinalIgnoreCase),
            $"La etapa final debe ejecutar con un usuario no root; " +
            $"se encontró '{usuario.Groups[1].Value}'.");
    }

    [Fact]
    [Trait("HU", "HU-31")]
    public void Dockerfile_DebeDeclararHealthcheckQueVerifiqueHealth()
    {
        var seccionFinal = ObtenerSeccionEtapaFinal(LeerDockerfile());

        Assert.True(
            seccionFinal.Contains("HEALTHCHECK", StringComparison.OrdinalIgnoreCase),
            "El Dockerfile debe declarar 'HEALTHCHECK' en la etapa final para Docker/Kubernetes.");

        Assert.True(
            seccionFinal.Contains("/health", StringComparison.OrdinalIgnoreCase),
            "El 'HEALTHCHECK' debe verificar el endpoint '/health'.");
    }

    private const string ImagenSdk = "mcr.microsoft.com/dotnet/sdk:9.0";
    private const string ImagenRuntime = "mcr.microsoft.com/dotnet/aspnet:9.0";

    private static string LeerDockerfile()
    {
        var ruta = Path.Combine(
            RaizRepositorio.Obtener(),
            "Dockerfile");

        Assert.True(
            File.Exists(ruta),
            $"Debe existir el Dockerfile multi-stage en {ruta}.");

        return File.ReadAllText(ruta);
    }

    private static (string Imagen, string Nombre)[] ExtraerEtapas(string contenido)
    {
        return Regex.Matches(
                contenido,
                @"^FROM\s+(?<imagen>\S+)(?:\s+AS\s+(?<nombre>\S+))?",
                RegexOptions.IgnoreCase | RegexOptions.Multiline)
            .Select(coincidencia => (
                Imagen: coincidencia.Groups["imagen"].Value,
                Nombre: coincidencia.Groups["nombre"].Success
                    ? coincidencia.Groups["nombre"].Value
                    : string.Empty))
            .ToArray();
    }

    private static string ObtenerSeccionEtapaFinal(string contenido)
    {
        var lineas = contenido
            .Split('\n')
            .Select(linea => linea.TrimEnd('\r'))
            .Where(linea => !linea.TrimStart().StartsWith('#'))
            .ToArray();

        var indiceUltimoFrom = Array.FindLastIndex(
            lineas,
            linea => linea.TrimStart()
                .StartsWith("FROM", StringComparison.OrdinalIgnoreCase));

        Assert.True(
            indiceUltimoFrom >= 0,
            "El Dockerfile debe contener al menos una instrucción FROM.");

        return string.Join(
            Environment.NewLine,
            lineas[indiceUltimoFrom..]);
    }
}
