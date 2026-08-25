using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Licitaciones.Api.Infraestructura;

/// <summary>
/// Fábrica personalizada de ProblemDetails para respuestas de error de la API.
/// </summary>
public sealed class FabricaProblemDetailsApi : ProblemDetailsFactory
{
    /// <summary>
    /// Crea un ProblemDetails con valores predeterminados según el estado HTTP.
    /// </summary>
    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var estado = statusCode ?? httpContext.Response.StatusCode;

        var problema = new ProblemDetails
        {
            Status = estado,
            Title = title ?? TituloPorDefecto(estado),
            Type = type ?? $"https://httpstatuses.com/{estado}",
            Detail = detail ?? DetallePorDefecto(estado),
            Instance = instance ?? httpContext.Request.Path
        };
        ContratoProblemasApi.AplicarExtensiones(httpContext, problema);

        return problema;
    }

    /// <summary>
    /// Crea un ValidationProblemDetails con errores de validación del modelo.
    /// </summary>
    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(modelStateDictionary);

        var estado = statusCode ?? StatusCodes.Status400BadRequest;

        var problema = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = estado,
            Title = title ?? TituloPorDefecto(estado),
            Type = type ?? $"https://httpstatuses.com/{estado}",
            Detail = detail ?? DetallePorDefecto(estado),
            Instance = instance ?? httpContext.Request.Path
        };
        ContratoProblemasApi.AplicarExtensiones(httpContext, problema);

        return problema;
    }

    private static string TituloPorDefecto(int estado) => estado switch
    {
        StatusCodes.Status400BadRequest => "Solicitud inválida",
        StatusCodes.Status404NotFound => "Recurso no encontrado",
        StatusCodes.Status409Conflict => "Conflicto con el estado actual",
        StatusCodes.Status422UnprocessableEntity => "Solicitud no procesable",
        _ => "Error al procesar la solicitud"
    };

    private static string DetallePorDefecto(int estado) => estado switch
    {
        StatusCodes.Status400BadRequest =>
            "La solicitud contiene datos inválidos. Revise los errores indicados.",
        StatusCodes.Status404NotFound => "El recurso solicitado no existe.",
        StatusCodes.Status409Conflict =>
            "La solicitud entra en conflicto con el estado actual del recurso.",
        StatusCodes.Status422UnprocessableEntity =>
            "La solicitud no pudo procesarse por una regla del negocio.",
        _ => "Ocurrió un error al procesar la solicitud."
    };
}
