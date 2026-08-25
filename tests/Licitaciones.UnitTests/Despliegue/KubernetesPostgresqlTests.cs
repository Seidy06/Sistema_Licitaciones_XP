using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Despliegue;

public sealed class KubernetesPostgresqlTests
{
    private static readonly string[] NombresManifiestoStatefulSet =
    {
        "postgres-statefulset.yaml",
        "postgresql-statefulset.yaml",
        "postgres.yaml"
    };

    private static readonly Regex PatronMigraciones = new(
        @"dotnet ef database update|ef\s+migrations\s+bundle|efbundle|migraci\w*|migrat\w*",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    [Fact]
    [Trait("HU", "HU-34")]
    public void Postgresql_DebeDefinirUnStatefulSetConVolumeClaimTemplates()
    {
        var contenido = LeerManifiesto(NombresManifiestoStatefulSet);

        Assert.True(
            Regex.IsMatch(contenido, @"kind:\s*StatefulSet"),
            "El manifiesto de PostgreSQL debe declarar un recurso de tipo "
            + "'StatefulSet' para conservar identidad y almacenamiento entre "
            + "reinicios de pods.");

        var plantillas = Regex.Match(
            contenido,
            @"volumeClaimTemplates:\s*\r?\n(?<cuerpo>(?:[ \t]+.+\r?\n?)+)");

        Assert.True(
            plantillas.Success,
            "El StatefulSet debe aprovisionar el PersistentVolumeClaim "
            + "mediante 'volumeClaimTemplates'.");

        var cuerpo = plantillas.Groups["cuerpo"].Value;

        foreach (var elemento in new[] { "metadata:", "spec:", "accessModes:", "storage:" })
        {
            Assert.True(
                cuerpo.Contains(elemento, StringComparison.Ordinal),
                $"El 'volumeClaimTemplates' debe declarar "
                + $"'{elemento.TrimEnd(':')}' para el PersistentVolumeClaim.");
        }

        Assert.True(
            Regex.IsMatch(cuerpo, @"accessModes:[\s\S]*?ReadWriteOnce"),
            "El PersistentVolumeClaim debe solicitar acceso 'ReadWriteOnce'.");
    }

    [Fact]
    [Trait("HU", "HU-34")]
    public void Postgresql_LosDatosDebenMontarseSobreElVolumenPersistente()
    {
        var contenido = LeerManifiesto(NombresManifiestoStatefulSet);

        var montaje = Regex.Match(
            contenido,
            @"name:\s*(?<nombre>[A-Za-z0-9_.\-]+)\s*\r?\n\s*mountPath:"
            + @"\s*/var/lib/postgresql/data");

        Assert.True(
            montaje.Success,
            "El contenedor de PostgreSQL debe montar el volumen persistente "
            + "en '/var/lib/postgresql/data' para que los datos sobrevivan al "
            + "reinicio del pod.");

        var nombre = montaje.Groups["nombre"].Value;

        var plantillas = Regex.Match(
            contenido,
            @"volumeClaimTemplates:\s*\r?\n(?<cuerpo>(?:[ \t]+.+\r?\n?)+)");

        Assert.True(
            plantillas.Success
            && plantillas.Groups["cuerpo"].Value.Contains(
                "name: " + nombre,
                StringComparison.Ordinal),
            $"El montaje '{nombre}' debe corresponder al nombre declarado en "
            + "'volumeClaimTemplates' para garantizar la persistencia.");
    }

    [Fact]
    [Trait("HU", "HU-34")]
    public void LasMigraciones_ClusterDebeAplicarlasDeFormaControlada()
    {
        var carpeta = Path.Combine(RaizRepositorio.Obtener(), "k8s");
        var mecanismoControlado = false;
        var manifiestosExistentes = Directory.Exists(carpeta)
            ? Directory.GetFiles(carpeta, "*.yaml")
            : Array.Empty<string>();

        foreach (var ruta in manifiestosExistentes)
        {
            var contenido = File.ReadAllText(ruta);

            var esJob = Regex.IsMatch(contenido, @"kind:\s*Job");
            var tieneInitContainers =
                Regex.IsMatch(contenido, @"initContainers:");

            if (!esJob && !tieneInitContainers)
            {
                continue;
            }

            var cuerpoEjecutable = contenido
                .Split('\n')
                .Select(linea => linea.TrimEnd('\r'))
                .Where(linea => !linea.TrimStart().StartsWith('#'));

            if (cuerpoEjecutable.Any(linea => PatronMigraciones.IsMatch(linea)))
            {
                mecanismoControlado = true;
                break;
            }
        }

        Assert.True(
            mecanismoControlado,
            "Las migraciones deben aplicarse de forma controlada mediante un "
            + "'Job' de Kubernetes (o 'initContainer') que ejecute "
            + "'dotnet ef database update' o el bundle de migraciones, antes "
            + "del despliegue de la aplicación.");
    }

    [Fact]
    [Trait("HU", "HU-34")]
    public void LasReplicasDeLaApi_NoDebenAplicarMigracionesAutomaticamente()
    {
        var despliegue = LeerManifiesto(new[] { "deployment.yaml" });
        var configuracion = LeerManifiesto(new[] { "configmap.yaml" });

        Assert.False(
            Regex.IsMatch(
                despliegue,
                @"Database__ApplyMigrationsOnStartup.*true"),
            "El Deployment de la API no debe activar "
            + "'Database__ApplyMigrationsOnStartup=true': cada réplica "
            + "aplicaría migraciones al arrancar.");

        Assert.False(
            Regex.IsMatch(
                configuracion,
                @"Database__ApplyMigrationsOnStartup:\s*""?true""?",
                RegexOptions.IgnoreCase),
            "El ConfigMap no debe activar "
            + "'Database__ApplyMigrationsOnStartup=true' para las réplicas.");
    }

    [Fact]
    [Trait("HU", "HU-34")]
    public void KubernetesMd_DebeDocumentarInstruccionesReproduciblesYLaEvidencia()
    {
        var ruta = Path.Combine(
            RaizRepositorio.Obtener(),
            "docs",
            "kubernetes.md");

        Assert.True(
            File.Exists(ruta),
            $"Debe existir la documentación de Kubernetes en {ruta}.");

        var contenido = File.ReadAllText(ruta);

        foreach (var elemento in new[]
                 {
                     "kubectl apply",
                     "statefulset",
                     "postgres"
                 })
        {
            Assert.True(
                contenido.Contains(elemento, StringComparison.OrdinalIgnoreCase),
                $"docs/kubernetes.md debe documentar '{elemento}' como parte "
                + "de las instrucciones reproducibles.");
        }

        Assert.True(
            Regex.IsMatch(contenido, @"get\s+(pods?|po)\b", RegexOptions.IgnoreCase),
            "docs/kubernetes.md debe incluir la evidencia de pods "
            + "(por ejemplo 'kubectl get pods').");

        Assert.True(
            Regex.IsMatch(contenido, @"get\s+(svc|service|services)\b", RegexOptions.IgnoreCase),
            "docs/kubernetes.md debe incluir la evidencia de servicios "
            + "(por ejemplo 'kubectl get svc').");

        Assert.True(
            Regex.IsMatch(contenido, @"get\s+pvc\b|persistentvolumeclaim", RegexOptions.IgnoreCase),
            "docs/kubernetes.md debe incluir la evidencia del "
            + "PersistentVolumeClaim (por ejemplo 'kubectl get pvc').");

        Assert.True(
            Regex.IsMatch(contenido, @"kubectl\s+logs|\blogs\b", RegexOptions.IgnoreCase),
            "docs/kubernetes.md debe incluir la evidencia de logs "
            + "(por ejemplo 'kubectl logs').");

        Assert.True(
            Regex.IsMatch(contenido, @"reinici\w*|restart", RegexOptions.IgnoreCase),
            "docs/kubernetes.md debe documentar la evidencia de conservación "
            + "de datos tras un reinicio del pod.");

        Assert.True(
            Regex.IsMatch(contenido, @"conserv\w*|persist\w*", RegexOptions.IgnoreCase),
            "docs/kubernetes.md debe dejar constancia de que los datos se "
            + "conservan o persisten entre reinicios.");
    }

    private static string LeerManifiesto(string[] nombresCandidatos)
    {
        var carpeta = Path.Combine(RaizRepositorio.Obtener(), "k8s");
        var rutaExistente = nombresCandidatos
            .Select(nombre => Path.Combine(carpeta, nombre))
            .FirstOrDefault(File.Exists);

        Assert.True(
            rutaExistente is not null,
            "Debe existir uno de los manifiestos "
            + string.Join(", ", nombresCandidatos.Select(nombre => $"'{nombre}'"))
            + $" dentro de '{carpeta}'.");

        return File.ReadAllText(rutaExistente!);
    }
}
