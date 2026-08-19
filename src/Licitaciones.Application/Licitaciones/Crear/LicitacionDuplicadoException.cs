namespace Licitaciones.Application.Licitaciones.Crear;

public sealed class LicitacionDuplicadoException : Exception
{
    public LicitacionDuplicadoException(string codigo)
        : base($"Ya existe una licitación con el código '{codigo}'.")
    {
    }
}
