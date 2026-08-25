using Licitaciones.E2ETests.Infraestructura;

using Microsoft.Playwright;

using static Microsoft.Playwright.Assertions;

namespace Licitaciones.E2ETests;

public sealed class ResponsividadMovilE2ETests : IClassFixture<LicitacionesE2EFixture>
{
    private const int AnchoMovil = 375;
    private const int AltoMovil = 667;

    private readonly LicitacionesE2EFixture _e2e;

    public ResponsividadMovilE2ETests(LicitacionesE2EFixture e2e) => _e2e = e2e;

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Landing_ConViewportMovil_DebeMostrarMainSinOverflow()
    {
        await using var contexto = await _e2e.Navegador.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = AnchoMovil, Height = AltoMovil }
        });
        var pagina = await contexto.NewPageAsync();

        await pagina.GotoAsync($"{_e2e.DireccionBase.TrimEnd('/')}/");

        await Expect(pagina.Locator("main")).ToBeVisibleAsync();
        var scrollWidth = await pagina.EvaluateAsync<int>("() => document.body.scrollWidth");
        Assert.True(
            scrollWidth <= AnchoMovil,
            $"En viewport móvil ({AnchoMovil}px), el body no debe desbordarse horizontalmente. scrollWidth = {scrollWidth}.");
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Licitaciones_ConViewportMovil_DebeMostrarMainSinOverflow()
    {
        await using var contexto = await _e2e.Navegador.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = AnchoMovil, Height = AltoMovil }
        });
        var pagina = await contexto.NewPageAsync();

        await pagina.GotoAsync($"{_e2e.DireccionBase.TrimEnd('/')}/Licitaciones");

        await Expect(pagina.Locator("main")).ToBeVisibleAsync();
        var scrollWidth = await pagina.EvaluateAsync<int>("() => document.body.scrollWidth");
        Assert.True(
            scrollWidth <= AnchoMovil,
            $"En viewport móvil ({AnchoMovil}px), el body no debe desbordarse horizontalmente. scrollWidth = {scrollWidth}.");
    }

    [Fact]
    [Trait("HU", "HU-30")]
    public async Task Proveedores_ConViewportMovil_DebeMostrarMainSinOverflow()
    {
        await using var contexto = await _e2e.Navegador.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = AnchoMovil, Height = AltoMovil }
        });
        var pagina = await contexto.NewPageAsync();

        await pagina.GotoAsync($"{_e2e.DireccionBase.TrimEnd('/')}/Proveedores");

        await Expect(pagina.Locator("main")).ToBeVisibleAsync();
        var scrollWidth = await pagina.EvaluateAsync<int>("() => document.body.scrollWidth");
        Assert.True(
            scrollWidth <= AnchoMovil,
            $"En viewport móvil ({AnchoMovil}px), el body no debe desbordarse horizontalmente. scrollWidth = {scrollWidth}.");
    }
}
