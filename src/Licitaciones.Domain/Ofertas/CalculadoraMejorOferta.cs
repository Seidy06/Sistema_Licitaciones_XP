namespace Licitaciones.Domain.Ofertas;

public static class CalculadoraMejorOferta
{
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

public sealed record ResultadoMejorOferta(
    Guid Id,
    decimal Monto,
    decimal AhorroPorcentaje,
    string Clasificacion);
