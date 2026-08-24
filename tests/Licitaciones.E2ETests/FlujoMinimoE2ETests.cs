using Licitaciones.E2ETests.Infraestructura;

using static Microsoft.Playwright.Assertions;

namespace Licitaciones.E2ETests;

public sealed class FlujoMinimoE2ETests : IClassFixture<LicitacionesE2EFixture>
{
    private readonly LicitacionesE2EFixture _e2e;

    public FlujoMinimoE2ETests(LicitacionesE2EFixture e2e) => _e2e = e2e;

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso01_Inicio_LaAplicacionDebeServirseEnNavegadorHeadless()
    {
        Assert.True(
            _e2e.NavegadorHeadless,
            "Las pruebas E2E deben ejecutarse contra la aplicación levantada con un navegador en modo headless.");

        await IrAsync("/");

        await Expect(_e2e.Pagina.Locator("main")).ToBeVisibleAsync();
        var titulo = await _e2e.Pagina.TitleAsync();
        Assert.Contains("Licitaciones", titulo, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso02_Proveedores_RegistrarDesdeFormularioWebDebePersistirYListarlo()
    {
        await IrAsync("/Proveedores/Create");

        await _e2e.Pagina.FillAsync("#Nombre", _e2e.NombreProveedorPrincipal);
        await _e2e.Pagina.ClickAsync("button[type=submit]");

        await Expect(_e2e.Pagina.Locator(".alert-success"))
            .ToContainTextAsync("El proveedor se registró correctamente.");

        await IrAsync("/Proveedores");
        var fila = _e2e.Pagina.Locator($"tr:has-text('{_e2e.NombreProveedorPrincipal}')");
        await Expect(fila).ToBeVisibleAsync();
        await Expect(fila.Locator("a:has-text('Ver detalle')")).ToBeVisibleAsync();

        var idProveedor = await _e2e.ObtenerProveedorIdPorNombreAsync(_e2e.NombreProveedorPrincipal);
        Assert.NotEqual(Guid.Empty, idProveedor);
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso03_Licitaciones_CrearDesdeFormularioWebDebePersistirYListarla()
    {
        var fechaCierre = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-ddTHH:mm");

        await IrAsync("/Licitaciones/Create");
        await _e2e.Pagina.FillAsync("#Codigo", _e2e.CodigoLicitacion);
        await _e2e.Pagina.FillAsync("#Titulo", _e2e.TituloLicitacion);
        await _e2e.Pagina.FillAsync("#Presupuesto", "10000");
        await _e2e.Pagina.FillAsync("#FechaCierre", fechaCierre);
        await _e2e.Pagina.ClickAsync("button[type=submit]");

        await IrAsync($"/Licitaciones?codigo={Uri.EscapeDataString(_e2e.CodigoLicitacion)}");

        var fila = _e2e.Pagina.Locator($"tr:has-text('{_e2e.TituloLicitacion}')");
        await Expect(fila).ToBeVisibleAsync();
        await Expect(fila).ToContainTextAsync("₡10.000,00");
        await Expect(fila).ToContainTextAsync("Borrador");

        var idLicitacion = await _e2e.ObtenerLicitacionIdPorCodigoAsync(_e2e.CodigoLicitacion);
        Assert.NotEqual(Guid.Empty, idLicitacion);
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso04_Licitaciones_PublicarDesdeElListadoWebDebeCambiarEstadoAPublicada()
    {
        await IrAsync($"/Licitaciones?codigo={Uri.EscapeDataString(_e2e.CodigoLicitacion)}");

        var fila = _e2e.Pagina.Locator($"tr:has-text('{_e2e.TituloLicitacion}')");
        await Expect(fila).ToBeVisibleAsync();

        await fila.Locator("[data-accion='publicar']").ClickAsync();
        await Expect(fila).ToContainTextAsync("Publicada");

        await IrAsync($"/Licitaciones?codigo={Uri.EscapeDataString(_e2e.CodigoLicitacion)}");
        await Expect(fila).ToContainTextAsync("Publicada");
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso05_Ofertas_RegistrarOfertaValidaDesdeFormularioWebDebeMostrarlaEnElListado()
    {
        var idLicitacion = await _e2e.ObtenerLicitacionIdPorCodigoAsync(_e2e.CodigoLicitacion);
        var idProveedor = await _e2e.ObtenerProveedorIdPorNombreAsync(_e2e.NombreProveedorPrincipal);

        await RegistrarOfertaAsync(idLicitacion, idProveedor, "9000");

        await Expect(_e2e.Pagina.Locator(".alert-success"))
            .ToContainTextAsync("La oferta se registró correctamente.");

        await IrAsync($"/Ofertas?licitacionId={idLicitacion}");
        var fila = _e2e.Pagina.Locator($"tr:has-text('{_e2e.NombreProveedorPrincipal}')");
        await Expect(fila).ToBeVisibleAsync();
        await Expect(fila).ToContainTextAsync("₡9.000,00");
        await Expect(fila).ToContainTextAsync("CRC");
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso06_Ofertas_VerificarRechazoDeOfertaDuplicadaYSobrePresupuesto()
    {
        var idLicitacion = await _e2e.ObtenerLicitacionIdPorCodigoAsync(_e2e.CodigoLicitacion);
        var idProveedor = await _e2e.ObtenerProveedorIdPorNombreAsync(_e2e.NombreProveedorPrincipal);

        await RegistrarOfertaAsync(idLicitacion, idProveedor, "9000");

        await Expect(_e2e.Pagina.Locator(".alert-danger"))
            .ToContainTextAsync("ya tiene una oferta activa para esta licitacion");

        await IrAsync("/Proveedores/Create");
        await _e2e.Pagina.FillAsync("#Nombre", _e2e.NombreProveedorSecundario);
        await _e2e.Pagina.ClickAsync("button[type=submit]");
        await Expect(_e2e.Pagina.Locator(".alert-success"))
            .ToContainTextAsync("El proveedor se registró correctamente.");
        var idProveedorSecundario =
            await _e2e.ObtenerProveedorIdPorNombreAsync(_e2e.NombreProveedorSecundario);

        await RegistrarOfertaAsync(idLicitacion, idProveedorSecundario, "99999");

        await Expect(_e2e.Pagina.Locator(".alert-danger"))
            .ToContainTextAsync("no puede superar el presupuesto de la licitacion");
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso07_Ofertas_ConsultarMejorOfertaDesdeLaInterfazWebDebeMostrarMontoYClasificacion()
    {
        var idLicitacion = await _e2e.ObtenerLicitacionIdPorCodigoAsync(_e2e.CodigoLicitacion);

        await IrAsync($"/Ofertas?licitacionId={idLicitacion}");

        var panelMejorOferta = _e2e.Pagina.Locator("[data-mejor-oferta]");
        await Expect(panelMejorOferta).ToBeVisibleAsync();
        await Expect(panelMejorOferta).ToContainTextAsync("₡9.000,00");
        await Expect(panelMejorOferta).ToContainTextAsync("Oferta conveniente");
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Paso08_Ofertas_AlternarMonedaEntreCRCyUSDDebeConvertirSinAlterarElValorOficial()
    {
        var idLicitacion = await _e2e.ObtenerLicitacionIdPorCodigoAsync(_e2e.CodigoLicitacion);

        await IrAsync($"/Ofertas?licitacionId={idLicitacion}");

        var selectorMoneda = _e2e.Pagina.Locator("select#moneda");
        await Expect(selectorMoneda).ToBeVisibleAsync();

        await selectorMoneda.SelectOptionAsync("CRC");
        await Expect(_e2e.Pagina.Locator("main")).ToContainTextAsync("₡9.000,00");

        await selectorMoneda.SelectOptionAsync("USD");
        var cuerpo = _e2e.Pagina.Locator("main");
        await Expect(cuerpo).ToContainTextAsync("USD");
        await Expect(cuerpo).Not.ToContainTextAsync("₡9.000,00");

        await selectorMoneda.SelectOptionAsync("CRC");
        await Expect(_e2e.Pagina.Locator("main")).ToContainTextAsync("₡9.000,00");
    }

    private async Task IrAsync(string ruta) =>
        await _e2e.Pagina.GotoAsync($"{_e2e.DireccionBase.TrimEnd('/')}{ruta}");

    private async Task RegistrarOfertaAsync(
        Guid idLicitacion,
        Guid idProveedor,
        string monto)
    {
        await IrAsync("/Ofertas/Create");
        await _e2e.Pagina.FillAsync("#LicitacionId", idLicitacion.ToString());
        await _e2e.Pagina.FillAsync("#ProveedorId", idProveedor.ToString());
        await _e2e.Pagina.FillAsync("#Monto", monto);
        await _e2e.Pagina.ClickAsync("button[type=submit]");
    }
}
