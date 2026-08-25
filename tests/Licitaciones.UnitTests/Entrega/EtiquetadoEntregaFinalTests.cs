using System.Diagnostics;
using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Entrega;

public sealed class EtiquetadoEntregaFinalTests
{
    private const string TagPrincipal = "v1.0.0";
    private const string TagAlternativo = "entrega-final";

    [Fact]
    [Trait("HU", "HU-37")]
    public void TagEntrega_DebeExistir_v100_o_entregaFinal_ApuntandoAlCommitFinalFuncional()
    {
        var raiz = RaizRepositorio.Obtener();

        var tags = EjecutarGit(raiz, "tag -l")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var tienePrincipal = tags.Any(t =>
            t.Equals(TagPrincipal, StringComparison.OrdinalIgnoreCase));

        var tieneAlternativo = tags.Any(t =>
            t.Equals(TagAlternativo, StringComparison.OrdinalIgnoreCase));

        Assert.True(
            tienePrincipal || tieneAlternativo,
            "El repositorio debe contener un tag '" + TagPrincipal + "' o '"
            + TagAlternativo + "' que identifique la entrega evaluable final "
            + "según el criterio de HU-37.");

        var nombreTag = tienePrincipal ? TagPrincipal : TagAlternativo;

        var (codigoRevParse, shaDelTag) = EjecutarGitConCodigo(
            raiz, "rev-parse " + nombreTag + "^{commit}");

        Assert.Equal(0, codigoRevParse);
        Assert.False(
            string.IsNullOrWhiteSpace(shaDelTag),
            "El tag '" + nombreTag + "' debe apuntar a un commit válido.");

        var (codigoAncestro, _) = EjecutarGitConCodigo(
            raiz, "merge-base --is-ancestor " + nombreTag + " HEAD");

        Assert.Equal(0, codigoAncestro);
    }

    [Fact]
    [Trait("HU", "HU-37")]
    public void HistorialCommits_DebeMostrarDistribucionEquilibradaConMensajesVinculadosAHistorias()
    {
        var raiz = RaizRepositorio.Obtener();

        var autores = EjecutarGit(raiz, "log --format=%aN main")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Assert.True(
            autores.Length > 0,
            "El historial de commits en main debe contener al menos un commit.");

        var seidy = autores.Count(a => a.Contains("Seidy", StringComparison.OrdinalIgnoreCase));
        var tiffany = autores.Count(a => a.Contains("Tiffany", StringComparison.OrdinalIgnoreCase));

        var total = autores.Length;
        var minimoPorAutor = (int)Math.Ceiling(total * 0.30);

        Assert.True(
            seidy >= minimoPorAutor,
            "Seidy tiene " + seidy + " de " + total + " commits en main; "
            + "se esperaba al menos el 30 % (" + minimoPorAutor
            + ") para cumplir la distribución equilibrada de HU-37.");

        Assert.True(
            tiffany >= minimoPorAutor,
            "Tiffany tiene " + tiffany + " de " + total + " commits en main; "
            + "se esperaba al menos el 30 % (" + minimoPorAutor
            + ") para cumplir la distribución equilibrada de HU-37.");

        var mensajes = EjecutarGit(raiz, "log --format=%s main")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var vinculados = mensajes.Count(m =>
            Regex.IsMatch(m, @"HU-\d+|refs\s+#\d+", RegexOptions.IgnoreCase));

        var umbralVinculacion = Math.Max(1, (int)Math.Floor(mensajes.Length * 0.60));

        Assert.True(
            vinculados >= umbralVinculacion,
            "De los " + mensajes.Length + " commits en main, solo " + vinculados
            + " están vinculados a historias (HU-## o refs #NN). El criterio de HU-37 "
            + "exige que la mayoría de los mensajes sean descriptivos y estén vinculados "
            + "a historias (umbral mínimo: " + umbralVinculacion + ").");
    }

    private static (int ExitCode, string Salida) EjecutarGitConCodigo(
        string directorioDeTrabajo, string argumentos)
    {
        var inicio = new ProcessStartInfo("git", argumentos)
        {
            WorkingDirectory = directorioDeTrabajo,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proceso = Process.Start(inicio)!;
        var salida = proceso.StandardOutput.ReadToEnd();
        proceso.WaitForExit();

        return (proceso.ExitCode, salida);
    }

    private static string EjecutarGit(string directorioDeTrabajo, string argumentos)
    {
        var (exitCode, salida) = EjecutarGitConCodigo(directorioDeTrabajo, argumentos);

        if (exitCode != 0)
        {
            throw new InvalidOperationException(
                "git " + argumentos + " falló con código " + exitCode + ".");
        }

        return salida;
    }
}
