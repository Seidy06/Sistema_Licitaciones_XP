using System.Text.RegularExpressions;

using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Despliegue;

public sealed class ComposeFileTests
{
    [Fact]
    [Trait("HU", "HU-32")]
    public void Compose_DebeDefinirLosServiciosAplicacionYBaseDeDatos()
    {
        var contenido = LeerCompose();

        var aplicacion = SeccionDeServicio(contenido, "app");
        var baseDatos = SeccionDeServicio(contenido, "db");

        Assert.True(
            aplicacion is not null,
            "El archivo Compose debe definir el servicio de aplicación 'app' "
            + "para levantar el entorno completo con 'docker compose up --build'.");
        Assert.True(
            baseDatos is not null,
            "El archivo Compose debe definir el servicio de PostgreSQL 'db'.");
    }

    [Fact]
    [Trait("HU", "HU-32")]
    public void Compose_AppDebeDefinirBuildParaConstruirseDesdeCero()
    {
        var aplicacion = RequerirSeccionDeServicio(LeerCompose(), "app");

        Assert.True(
            Regex.IsMatch(aplicacion, @"^\s*build:", RegexOptions.Multiline),
            "'docker compose up --build' exige que el servicio 'app' declare "
            + "la sección 'build' para construir la imagen desde cero.");
    }

    [Fact]
    [Trait("HU", "HU-32")]
    public void Compose_AppDebeDependerDeDbSaludable()
    {
        var aplicacion = RequerirSeccionDeServicio(LeerCompose(), "app");

        Assert.True(
            Regex.IsMatch(
                aplicacion,
                @"depends_on:\s*\n\s*db:\s*\n\s*condition:\s*service_healthy"),
            "El servicio 'app' debe declarar 'depends_on' sobre 'db' con "
            + "'condition: service_healthy' para esperar a PostgreSQL listo.");
    }

    [Fact]
    [Trait("HU", "HU-32")]
    public void Compose_LaAplicacionDebeContemplarMigracionesAutomaticasOJobDeInicializacion()
    {
        var contenido = LeerCompose();

        var mecanismo = Lineas(contenido)
            .Where(linea => !linea.TrimStart().StartsWith('#'))
            .Any(linea => PatronMigraciones.IsMatch(linea));

        Assert.True(
            mecanismo,
            "El entorno debe aplicar migraciones automáticamente (por ejemplo "
            + "'Database__ApplyMigrationsOnStartup=true') o mediante un job de "
            + "inicialización declarado en el Compose.");
    }

    [Fact]
    [Trait("HU", "HU-32")]
    public void Compose_DbDebeUsarImagenPostgres16ConVariablesEntornoYHealthcheck()
    {
        var baseDatos = RequerirSeccionDeServicio(LeerCompose(), "db");

        var imagen = Regex.Match(
                baseDatos,
                @"^\s*image:\s*(?<imagen>\S+)",
                RegexOptions.Multiline)
            .Groups["imagen"]
            .Value;

        Assert.True(
            imagen.EndsWith("postgres:16", StringComparison.OrdinalIgnoreCase),
            "El servicio 'db' debe usar la imagen 'postgres:16'; "
            + $"se encontró '{imagen}'.");

        foreach (var variable in new[]
                 {
                     "${POSTGRES_USER",
                     "${POSTGRES_PASSWORD",
                     "${POSTGRES_DB"
                 })
        {
            Assert.True(
                baseDatos.Contains(variable, StringComparison.Ordinal),
                $"Las credenciales de 'db' deben provenir del .env mediante "
                + $"'{variable}...'.");
        }

        Assert.True(
            baseDatos.Contains("pg_isready", StringComparison.Ordinal),
            "El servicio 'db' debe declarar un healthcheck con 'pg_isready'.");
    }

    [Fact]
    [Trait("HU", "HU-32")]
    public void Compose_VolumenNombradoDebeGarantizarPersistenciaTrasReinicio()
    {
        var contenido = LeerCompose();
        var baseDatos = RequerirSeccionDeServicio(contenido, "db");

        var montaje = Regex.Match(
            baseDatos,
            @"(?<nombre>[A-Za-z0-9_.\-]+):/var/lib/postgresql/data");

        Assert.True(
            montaje.Success,
            "El servicio 'db' debe montar un volumen en '/var/lib/postgresql/data'.");

        var nombre = montaje.Groups["nombre"].Value;

        Assert.False(
            nombre.StartsWith('$'),
            "Los datos deben persistir en un volumen nombrado, no en un montaje "
            + "dinámico ni de host.");

        var seccionVolumenes = SeccionSuperior(contenido, "volumes");

        Assert.True(
            Regex.IsMatch(
                seccionVolumenes,
                $@"^  {Regex.Escape(nombre)}:",
                RegexOptions.Multiline),
            $"El volumen nombrado '{nombre}' debe estar declarado en la sección "
            + "superior 'volumes:' para sobrevivir a reinicios y recreaciones.");
    }

    private static readonly Regex PatronMigraciones = new(
        @"ApplyMigrationsOnStartup=true|dotnet ef database update|migraci\w*|migrat\w*",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static string LeerCompose()
    {
        var raiz = RaizRepositorio.Obtener();
        var ruta = Path.Combine(raiz, "compose.yaml");

        if (!File.Exists(ruta))
        {
            ruta = Path.Combine(raiz, "docker-compose.yml");
        }

        Assert.True(
            File.Exists(ruta),
            $"Debe existir el archivo Compose ({Path.Combine(raiz, "compose.yaml")} "
            + $"o {Path.Combine(raiz, "docker-compose.yml")}).");

        return File.ReadAllText(ruta);
    }

    private static string RequerirSeccionDeServicio(string contenido, string servicio)
    {
        var seccion = SeccionDeServicio(contenido, servicio);

        Assert.True(
            seccion is not null,
            $"El archivo Compose debe definir el servicio '{servicio}'.");

        return seccion!;
    }

    private static string? SeccionDeServicio(string contenido, string servicio)
    {
        var lineas = Lineas(contenido).ToArray();

        var patronInicio = new Regex($@"^  {Regex.Escape(servicio)}:\s*$");
        var inicio = Array.FindIndex(
            lineas,
            linea => patronInicio.IsMatch(linea));

        if (inicio < 0)
        {
            return null;
        }

        var fin = inicio + 1;
        while (fin < lineas.Length && !Regex.IsMatch(lineas[fin], @"^  \S"))
        {
            fin++;
        }

        return string.Join("\n", lineas[inicio..fin]);
    }

    private static string? SeccionSuperior(string contenido, string clave)
    {
        var lineas = Lineas(contenido).ToArray();

        var patronInicio = new Regex($@"^{Regex.Escape(clave)}:\s*$");
        var inicio = Array.FindIndex(lineas, linea => patronInicio.IsMatch(linea));

        if (inicio < 0)
        {
            return null;
        }

        var fin = inicio + 1;
        while (
            fin < lineas.Length
            && (lineas[fin].StartsWith(' ') || lineas[fin].StartsWith('\t')))
        {
            fin++;
        }

        return string.Join("\n", lineas[inicio..fin]);
    }

    private static IEnumerable<string> Lineas(string contenido)
    {
        return contenido
            .Split('\n')
            .Select(linea => linea.TrimEnd('\r'));
    }
}
