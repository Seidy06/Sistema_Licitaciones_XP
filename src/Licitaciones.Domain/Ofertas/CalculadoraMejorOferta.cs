namespace Licitaciones.Domain.Ofertas;

/// <summary>
/// Servicio de dominio que determina la mejor oferta entre un conjunto de ofertas para una licitación.
/// </summary>
public static class CalculadoraMejorOferta
{
    /// <summary>
    /// Calcula la mejor oferta (menor monto) y su porcentaje de ahorro respecto al presupuesto.
    /// </summary>
    /// <param name="presupuesto">Presupuesto de referencia de la licitación.</param>
    /// <param name="ofertas">Conjunto de ofertas a evaluar.</param>
    /// <returns>Resultado con la mejor oferta y su clasificación, o null si no hay ofertas.</returns>
    public static ResultadoMejorOferta? Calcular(
        decimal presupuesto,
        IEnumerable<Oferta> ofertas)
    {
        var mejorOferta = ofertas
            .OrderBy(oferta => oferta.Monto)
            .ThenBy(oferta => oferta.FechaRegistro)
            .FirstOrDefault();

        if (mejorOferta is null)
        {
            return null;
        }

        var ahorroPorcentaje =
            (presupuesto - mejorOferta.Monto) / presupuesto * 100m;

        var clasificacion = ahorroPorcentaje switch
        {
            >= 10m => "Oferta conveniente",
            > 0m => "Oferta aceptable",
            _ => "Oferta válida sin ahorro"
        };

        return new ResultadoMejorOferta(
            mejorOferta.Id,
            mejorOferta.Monto,
            ahorroPorcentaje,
            clasificacion);
    }
}

/// <summary>
/// Resultado del cálculo de la mejor oferta, incluyendo ahorro y clasificación.
/// </summary>
/// <param name="Id">Identificador de la mejor oferta.</param>
/// <param name="Monto">Monto de la mejor oferta.</param>
/// <param name="AhorroPorcentaje">Porcentaje de ahorro respecto al presupuesto.</param>
/// <param name="Clasificacion">Clasificación cualitativa de la oferta.</param>
public sealed record ResultadoMejorOferta(
    Guid Id,
    decimal Monto,
    decimal AhorroPorcentaje,
    string Clasificacion);
