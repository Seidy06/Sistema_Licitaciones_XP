using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Despliegue;

public sealed class PipelineCiTests
{
    private static readonly Regex PatronCobertura = new(
        @"--collect[^""]*XPlat\s+Code\s+Coverage|--collect:""XPlat Code Coverage""",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static string LeerWorkflow()
    {
        var ruta = Path.Combine(
            RaizRepositorio.Obtener(),
            ".github",
            "workflows",
            "ci.yml");

        Assert.True(
            File.Exists(ruta),
            "Debe existir el workflow de integración continua en "
            + "'.github/workflows/ci.yml' para disparar el pipeline en cada "
            + "push o pull request.");

        return File.ReadAllText(ruta);
    }

    private static int IndiceDe(string contenido, string aguja, string descripcion)
    {
        var indice = contenido.IndexOf(aguja, StringComparison.OrdinalIgnoreCase);

        Assert.True(
            indice >= 0,
            $"El pipeline debe incluir {descripcion} ('{aguja}') para cumplir "
            + "el criterio de HU-35.");

        return indice;
    }

    [Fact]
    [Trait("HU", "HU-35")]
    public void ElPipeline_DebeEjecutarRestoreBuildYPruebasConCoberturaEnOrden()
    {
        var contenido = LeerWorkflow();

        var disparoPullRequest = Regex.IsMatch(
            contenido,
            @"pull_request\s*:",
            RegexOptions.IgnoreCase);

        Assert.True(
            disparoPullRequest,
            "El workflow debe dispararse también en 'pull_request' para que "
            + "sus resultados bloqueen la integración de cambios.");

        var restore = IndiceDe(contenido, "dotnet restore", "la restauración");
        var build = IndiceDe(contenido, "dotnet build", "la compilación");
        var test = IndiceDe(contenido, "dotnet test", "la ejecución de pruebas");

        Assert.True(
            restore < build && build < test,
            "El pipeline debe ejecutar en orden: restore, luego build y "
            + "luego test.");

        Assert.True(
            PatronCobertura.IsMatch(contenido),
            "'dotnet test' debe recolectar cobertura (por ejemplo "
            + "--collect \"XPlat Code Coverage\") según el criterio de HU-35.");
    }

    [Fact]
    [Trait("HU", "HU-35")]
    public void ElPipeline_DebeIncluirAnalisisEstaticoOVisionadoDeFormato()
    {
        var contenido = LeerWorkflow();

        Assert.True(
            Regex.IsMatch(
                contenido,
                @"dotnet\s+format[\s\S]{0,120}--verify-no-changes",
                RegexOptions.IgnoreCase),
            "El pipeline debe validar el formato con "
            + "'dotnet format --verify-no-changes' (análisis estático/formato).");
    }

    [Fact]
    [Trait("HU", "HU-35")]
    public void ElPipeline_DebeConstruirLaImagenDockerDespuesDeLasPruebas()
    {
        var contenido = LeerWorkflow();
        var test = IndiceDe(contenido, "dotnet test", "la ejecución de pruebas");

        var dockerBuild = Regex.Match(
            contenido,
            @"docker\s+build[^\n]*|uses:\s*docker/build-push-action",
            RegexOptions.IgnoreCase);

        Assert.True(
            dockerBuild.Success,
            "El pipeline debe construir la imagen Docker (por ejemplo "
            + "'docker build -f Dockerfile .') sin publicarla.");

        Assert.True(
            contenido.IndexOf(dockerBuild.Value, StringComparison.OrdinalIgnoreCase) > test,
            "El build de la imagen Docker debe ir después de las pruebas "
            + "según el orden del criterio de HU-35.");
    }

    [Fact]
    [Trait("HU", "HU-35")]
    public void ElPipeline_DebeValidarLosManifiestosDeKubernetes()
    {
        var contenido = LeerWorkflow();
        var dockerBuildIndice = Regex.Match(
            contenido,
            @"docker\s+build[^\n]*|uses:\s*docker/build-push-action",
            RegexOptions.IgnoreCase);
        var docker = dockerBuildIndice.Success
            ? contenido.IndexOf(dockerBuildIndice.Value, StringComparison.OrdinalIgnoreCase)
            : -1;

        var validacion = Regex.Match(
            contenido,
            @"kubeconform|kubeval|kubectl\s+apply\s+[^&\n]*--dry-run\s*=\s*(client|server)",
            RegexOptions.IgnoreCase);

        Assert.True(
            validacion.Success,
            "El pipeline debe validar los manifiestos de 'k8s/' (por ejemplo "
            + "kubeconform o 'kubectl apply --dry-run=client').");

        var indiceValidacion = contenido.IndexOf(validacion.Value, StringComparison.OrdinalIgnoreCase);

        Assert.True(
            docker < 0 || indiceValidacion > docker,
            "La validación de manifiestos K8s debe ir después del build de "
            + "la imagen Docker según el orden del criterio.");
    }

    [Fact]
    [Trait("HU", "HU-35")]
    public void ElPipeline_DebeAuditarDependenciasVulnerablesAlFinal()
    {
        var contenido = LeerWorkflow();

        var auditoria = IndiceDe(
            contenido,
            "dotnet list package --vulnerable",
            "la auditoría de dependencias vulnerables");

        var test = IndiceDe(contenido, "dotnet test", "la ejecución de pruebas");

        Assert.True(
            auditoria > test,
            "La auditoría de dependencias debe ejecutarse al final del "
            + "pipeline, después de las pruebas.");
    }

    [Fact]
    [Trait("HU", "HU-35")]
    public void ElPipeline_NoDebeTolerarFallosEnNingunPaso()
    {
        var contenido = LeerWorkflow();

        foreach (var patron in new[]
                 {
                     new Regex(@"continue-on-error\s*:\s*true", RegexOptions.IgnoreCase),
                     new Regex(@"\|\|\s*true\b"),
                     new Regex(@"exit\s+0\b")
                 })
        {
            Assert.False(
                patron.IsMatch(contenido),
                "Ningún paso del pipeline puede ignorar fallos ("
                + patron.ToString() + "): cualquier paso fallido debe romper "
                + "el workflow y bloquear el merge.");
        }
    }
}
