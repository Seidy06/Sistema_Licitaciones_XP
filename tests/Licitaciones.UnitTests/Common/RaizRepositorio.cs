namespace Licitaciones.UnitTests.Common;

public static class RaizRepositorio
{
    public static string Obtener()
    {
        var directorio = AppContext.BaseDirectory;

        while (directorio is not null
               && !File.Exists(Path.Combine(directorio, "docs", "historias-usuario.md")))
        {
            directorio = Path.GetDirectoryName(directorio.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar));
        }

        Assert.NotNull(directorio);

        return directorio!;
    }
}
