using System.Reflection;
using System.Text.Json;

using Licitaciones.Api.Infraestructura;
using Licitaciones.Application.Aprobaciones;
using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Cerrar;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Application.Licitaciones.Publicar;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Ofertas.Editar;
using Licitaciones.Application.Ofertas.Eliminar;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Application.Proveedores.Eliminar;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Common;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Infrastructure.Time;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Licitaciones API",
        Version = "v1",
        Description = "API REST del sistema de licitaciones: proveedores, "
            + "licitaciones, ofertas, niveles de aprobación y tipos de cambio."
    });

    var rutaXml = Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(rutaXml))
    {
        options.IncludeXmlComments(rutaXml);
    }

    options.SchemaFilter<EjemplosEsquemasFiltro>();
});
builder.Services.AddSingleton<ProblemDetailsFactory, FabricaProblemDetailsApi>();
builder.Services.AddScoped<AdministrarNivelesAprobacionService>();
builder.Services.AddScoped<ResolverNivelAprobacionService>();
builder.Services.AddScoped<INivelAprobacionRepository, NivelAprobacionRepository>();
builder.Services.AddScoped<CrearProveedorService>();
builder.Services.AddScoped<ConsultarProveedorService>();
builder.Services.AddScoped<EditarProveedorService>();
builder.Services.AddScoped<DarBajaProveedorService>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorConsultaRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorBajaRepository, ProveedorRepository>();
builder.Services.AddScoped<CrearLicitacionService>();
builder.Services.AddScoped<ConsultarLicitacionService>();
builder.Services.AddScoped<EditarLicitacionService>();
builder.Services.AddScoped<PublicarLicitacionService>();
builder.Services.AddScoped<CerrarLicitacionService>();
builder.Services.AddScoped<ILicitacionRepository, LicitacionRepository>();
builder.Services.AddScoped<ILicitacionConsultaRepository, LicitacionConsultaRepository>();
builder.Services.AddScoped<CrearOfertaService>();
builder.Services.AddScoped<ConsultarOfertaService>();
builder.Services.AddScoped<EditarOfertaService>();
builder.Services.AddScoped<EliminarOfertaService>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<IOfertaConsultaRepository, OfertaRepository>();
builder.Services.AddScoped<IEditarOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<IEliminarOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<AdministrarTipoCambioService>();
builder.Services.AddScoped<ITipoCambioRepository, TipoCambioRepository>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddDbContext<LicitacionesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Licitaciones")));

var app = builder.Build();

if (builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", false))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<LicitacionesDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Licitaciones API v1"));
}

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandler = async contexto =>
    {
        var excepcion = contexto.Features.Get<IExceptionHandlerFeature>()?.Error;
        var esReglaNegocio = excepcion is DomainException;
        var estado = esReglaNegocio
            ? StatusCodes.Status422UnprocessableEntity
            : StatusCodes.Status500InternalServerError;

        var fabrica = contexto.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();
        var problema = fabrica.CreateProblemDetails(
            contexto,
            estado,
            esReglaNegocio ? "Solicitud no procesable" : "Error interno del servidor",
            detail: esReglaNegocio
                ? "La solicitud no pudo procesarse por una regla del negocio."
                : "Ocurrió un error interno inesperado. Intente nuevamente más tarde.");
        ContratoProblemasApi.AplicarExtensiones(
            contexto,
            problema,
            esReglaNegocio ? "regla_negocio_no_procesable" : "error_interno");

        contexto.Response.StatusCode = estado;
        contexto.Response.ContentType = RespuestaProblema.TipoContenido;

        await contexto.Response.WriteAsync(
            JsonSerializer.Serialize(problema, OpcionesJsonHttp.Instancia));
    }
});

app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (contexto, reporte) =>
        await contexto.Response.WriteAsync(reporte.Status.ToString())
});

app.MapControllers();

app.Run();

internal static class OpcionesJsonHttp
{
    public static readonly JsonSerializerOptions Instancia =
        new(JsonSerializerDefaults.Web);
}

public partial class Program;
