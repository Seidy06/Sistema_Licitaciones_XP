using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Infraestructura;

/// <summary>
/// Utilidad estática para aplicar extensiones estándar a ProblemDetails de la API.
/// </summary>
public static class ContratoProblemasApi
{
    /// <summary>Clave para el código de error en las extensiones de ProblemDetails.</summary>
    public const string ClaveCodigoError = "codigoError";

    /// <summary>Clave para el identificador de correlación en las extensiones de ProblemDetails.</summary>
    public const string ClaveCorrelacionId = "correlacionId";

    /// <summary>
    /// Aplica las extensiones de código de error y correlación a un ProblemDetails.
    /// </summary>
    public static void AplicarExtensiones(
        HttpContext contexto,
        ProblemDetails problema,
        string? codigoError = null)
    {
        problema.Extensions[ClaveCodigoError] =
            string.IsNullOrWhiteSpace(codigoError)
                ? $"error_http_{problema.Status ?? StatusCodes.Status500InternalServerError}"
                : codigoError;
        problema.Extensions[ClaveCorrelacionId] = contexto.TraceIdentifier;
    }
}
