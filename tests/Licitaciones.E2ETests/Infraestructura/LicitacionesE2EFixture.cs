using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using Licitaciones.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using Microsoft.Playwright;

using Testcontainers.PostgreSql;

namespace Licitaciones.E2ETests.Infraestructura;

public sealed class LicitacionesE2EFixture : IAsyncLifetime
{
    private const string VariableEntornoCanalNavegador = "LICITACIONES_E2E_BROWSER_CHANNEL";
    private const int TiempoMaximoEsperaSegundos = 120;

    private readonly PostgreSqlContainer _contenedor;

    private Process? _procesoAplicacion;
    private IPlaywright? _playwright;
    private IBrowser? _navegador;
    private IBrowserContext? _contexto;

    public LicitacionesE2EFixture()
    {
        _contenedor = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("licitaciones_db")
            .WithUsername("licitaciones_user")
            .WithPassword("licitaciones_password")
            .Build();

        var sufijo = Guid.NewGuid().ToString("N")[..8];
        NombreProveedorPrincipal = $"Proveedor E2E HU30 principal {sufijo}";
        NombreProveedorSecundario = $"Proveedor E2E HU30 secundario {sufijo}";
        CodigoLicitacion = $"E2E-HU30-{sufijo}";
        TituloLicitacion = "Compra de equipos para pruebas E2E HU-30";
    }

    public string DireccionBase { get; private set; } = string.Empty;

    public IPage Pagina { get; private set; } = null!;

    public bool NavegadorHeadless { get; private set; }

    public string NombreProveedorPrincipal { get; }

    public string NombreProveedorSecundario { get; }

    public string CodigoLicitacion { get; }

    public string TituloLicitacion { get; }

    public async Task InitializeAsync()
    {
        await _contenedor.StartAsync();

        var puerto = ObtenerPuertoLibre();
        DireccionBase = $"http://127.0.0.1:{puerto}/";

        _procesoAplicacion = IniciarAplicacion(puerto);
        await EsperarAplicacionLevantadaAsync();

        _playwright = await Playwright.CreateAsync();
        _navegador = await LanzarNavegadorHeadlessAsync();
        NavegadorHeadless = true;
        _contexto = await _navegador.NewContextAsync();
        _contexto.SetDefaultTimeout(15_000);
        Pagina = await _contexto.NewPageAsync();
    }

    public async Task<Guid> ObtenerLicitacionIdPorCodigoAsync(string codigo)
    {
        await using var contexto = CrearContexto();
        return await contexto.Licitaciones
            .Where(licitacion => licitacion.Codigo == codigo)
            .Select(licitacion => licitacion.Id)
            .SingleAsync();
    }

    public async Task<Guid> ObtenerProveedorIdPorNombreAsync(string nombre)
    {
        await using var contexto = CrearContexto();
        return await contexto.Proveedores
            .Where(proveedor => proveedor.Nombre == nombre)
            .Select(proveedor => proveedor.Id)
            .SingleAsync();
    }

    public async Task DisposeAsync()
    {
        if (_contexto is not null)
        {
            await _contexto.CloseAsync();
        }

        if (_navegador is not null)
        {
            await _navegador.CloseAsync();
        }

        _playwright?.Dispose();

        if (_procesoAplicacion is not null && !_procesoAplicacion.HasExited)
        {
            _procesoAplicacion.Kill(entireProcessTree: true);
        }

        _procesoAplicacion?.Dispose();

        await _contenedor.DisposeAsync();
    }

    private static int ObtenerPuertoLibre()
    {
        var escucha = new TcpListener(IPAddress.Loopback, 0);
        escucha.Start();
        try
        {
            return ((IPEndPoint)escucha.LocalEndpoint).Port;
        }
        finally
        {
            escucha.Stop();
        }
    }

    private Process IniciarAplicacion(int puerto)
    {
        var rutaDll = ResolverRutaDllAplicacion();
        var inicio = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{rutaDll}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        inicio.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";
        inicio.EnvironmentVariables["ASPNETCORE_URLS"] = $"http://127.0.0.1:{puerto}";
        inicio.EnvironmentVariables["ConnectionStrings__Licitaciones"] =
            _contenedor.GetConnectionString();
        inicio.EnvironmentVariables["Database__ApplyMigrationsOnStartup"] = "true";

        return Process.Start(inicio)
            ?? throw new InvalidOperationException("No fue posible levantar el proceso de la aplicación.");
    }

    private static string ResolverRutaDllAplicacion()
    {
        var directorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (directorio is not null && !File.Exists(Path.Combine(directorio.FullName, "Licitaciones.sln")))
        {
            directorio = directorio.Parent;
        }

        if (directorio is null)
        {
            throw new InvalidOperationException(
                "No se encontró la raíz de la solución desde el directorio de pruebas.");
        }

        return Path.Combine(
            directorio.FullName,
            "src", "Licitaciones.Web", "bin", "Release", "net9.0", "Licitaciones.Web.dll");
    }

    private async Task EsperarAplicacionLevantadaAsync()
    {
        var limite = DateTime.UtcNow.AddSeconds(TiempoMaximoEsperaSegundos);
        using var cliente = new HttpClient();

        while (DateTime.UtcNow < limite)
        {
            if (_procesoAplicacion!.HasExited)
            {
                throw new InvalidOperationException(
                    $"El proceso de la aplicación terminó inesperadamente con código {_procesoAplicacion.ExitCode}.");
            }

            try
            {
                using var respuesta = await cliente.GetAsync(DireccionBase);
                if (respuesta.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }

            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"La aplicación no quedó disponible en '{DireccionBase}' tras {TiempoMaximoEsperaSegundos} segundos.");
    }

    private LicitacionesDbContext CrearContexto() =>
        new(new DbContextOptionsBuilder<LicitacionesDbContext>()
            .UseNpgsql(_contenedor.GetConnectionString())
            .Options);

    private async Task<IBrowser> LanzarNavegadorHeadlessAsync()
    {
        var canalConfigurado = Environment.GetEnvironmentVariable(VariableEntornoCanalNavegador);
        var candidatos = string.IsNullOrWhiteSpace(canalConfigurado)
            ? new string?[] { null, "msedge", "chrome" }
            : [canalConfigurado];

        Exception? ultimoError = null;
        foreach (var candidato in candidatos)
        {
            try
            {
                return await _playwright!.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Channel = candidato,
                    Headless = true
                });
            }
            catch (PlaywrightException error)
            {
                ultimoError = error;
            }
        }

        throw ultimoError
            ?? new PlaywrightException("No fue posible lanzar un navegador Chromium en modo headless.");
    }
}
