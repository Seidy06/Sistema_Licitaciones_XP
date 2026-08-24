using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Despliegue;

public sealed class KubernetesManifestsTests
{
    private static readonly string[] ArchivosManifiesto =
    {
        "deployment.yaml",
        "service.yaml",
        "configmap.yaml",
        "secret.yaml"
    };

    [Fact]
    [Trait("HU", "HU-33")]
    public void CarpetaK8s_DebeContenerLosCuatroManifiestosDeLaAplicacion()
    {
        foreach (var archivo in ArchivosManifiesto)
        {
            Assert.True(
                File.Exists(Ruta(archivo)),
                $"Debe existir el manifiesto '{archivo}' dentro de la carpeta "
                + "'k8s' para desplegar la aplicación en Kubernetes.");
        }
    }

    [Fact]
    [Trait("HU", "HU-33")]
    public void Deployment_DebeDefinirStartupReadinessYLivenessProbes()
    {
        var contenido = LeerManifiesto("deployment.yaml");

        foreach (var sonda in new[]
                 {
                     "startupProbe:",
                     "readinessProbe:",
                     "livenessProbe:"
                 })
        {
            Assert.True(
                contenido.Contains(sonda, StringComparison.Ordinal),
                $"El Deployment debe definir '{sonda.TrimEnd(':')}' para que el "
                + "clúster supervise el ciclo de vida del contenedor de la aplicación.");
        }
    }

    [Fact]
    [Trait("HU", "HU-33")]
    public void Deployment_DebeDefinirResourcesConRequestsYLimits()
    {
        var contenido = LeerManifiesto("deployment.yaml");

        var recursos = Regex.Match(
            contenido,
            @"resources:\s*\r?\n(?<cuerpo>(?:[ \t]+.+\r?\n?)+)");

        Assert.True(
            recursos.Success,
            "El contenedor del Deployment debe declarar la sección 'resources'.");

        var cuerpo = recursos.Groups["cuerpo"].Value;

        foreach (var seccion in new[] { "requests:", "limits:" })
        {
            Assert.True(
                cuerpo.Contains(seccion, StringComparison.Ordinal),
                $"La sección 'resources' debe definir '{seccion.TrimEnd(':')}' "
                + "de cpu y memoria.");
        }

        foreach (var seccion in new[] { "requests", "limits" })
        {
            foreach (var recurso in new[] { "cpu:", "memory:" })
            {
                Assert.True(
                    Regex.IsMatch(
                        cuerpo,
                        $@"{seccion}:[\s\S]*?{recurso}"),
                    $"La sección '{seccion}' debe declarar '{recurso.TrimEnd(':')}'.");
            }
        }
    }

    [Fact]
    [Trait("HU", "HU-33")]
    public void Deployment_DebeObtenerLasCredencialesDesdeUnSecret()
    {
        var contenido = LeerManifiesto("deployment.yaml");

        Assert.True(
            contenido.Contains("secretKeyRef", StringComparison.Ordinal),
            "Las credenciales de base de datos deben provenir de un Secret "
            + "mediante 'secretKeyRef', nunca de literales en el manifiesto.");
    }

    [Fact]
    [Trait("HU", "HU-33")]
    public void LosManifiestos_NoDebenContenerContrasenasHardcodeadas()
    {
        foreach (var archivo in ArchivosManifiesto)
        {
            var contenido = LeerManifiesto(archivo);

            foreach (var credencial in new[]
                     {
                         "change_this_password",
                         "licitaciones_password"
                     })
            {
                Assert.False(
                    contenido.Contains(credencial, StringComparison.OrdinalIgnoreCase),
                    $"'{archivo}' no debe contener la contraseña literal "
                    + $"'{credencial}': las credenciales provienen de un Secret.");
            }
        }
    }

    [Fact]
    [Trait("HU", "HU-33")]
    public void Service_DebeExponerElPuertoDeLaAplicacionDentroDelCluster()
    {
        var contenido = LeerManifiesto("service.yaml");

        Assert.True(
            Regex.IsMatch(contenido, @"kind:\s*Service"),
            "El manifiesto 'service.yaml' debe declarar un recurso de tipo 'Service'.");

        Assert.True(
            Regex.IsMatch(contenido, @"targetPort:\s*8080"),
            "El Service debe exponer el puerto interno de la aplicación (8080) "
            + "dentro del clúster.");
    }

    private static string Ruta(string archivo) =>
        Path.Combine(RaizRepositorio.Obtener(), "k8s", archivo);

    private static string LeerManifiesto(string archivo)
    {
        var ruta = Ruta(archivo);

        Assert.True(
            File.Exists(ruta),
            $"Debe existir el manifiesto '{archivo}' en {ruta}.");

        return File.ReadAllText(ruta);
    }
}
