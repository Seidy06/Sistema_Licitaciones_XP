namespace Licitaciones.Application.Ofertas.Editar;

public sealed record EditarOfertaRequest(
    Guid Id,
    decimal Monto);
