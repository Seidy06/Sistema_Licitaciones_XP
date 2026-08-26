using System.Diagnostics;

using Licitaciones.Web.Models;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

/// <summary>
/// Controlador principal con las páginas de inicio, privacidad y errores del sistema.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Inicializa una nueva instancia del controlador principal.
    /// </summary>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Muestra la página de inicio del sistema.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Muestra la política de privacidad de la aplicación.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Muestra la página de error cuando ocurre una excepción no controlada.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
