using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Application.Proveedores.Eliminar;
using Licitaciones.Domain.Common;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Infrastructure.Time;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddScoped<CrearProveedorService>();
builder.Services.AddScoped<ConsultarProveedorService>();
builder.Services.AddScoped<EditarProveedorService>();
builder.Services.AddScoped<DarBajaProveedorService>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorConsultaRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorBajaRepository, ProveedorRepository>();
builder.Services.AddScoped<CrearLicitacionService>();
builder.Services.AddScoped<ILicitacionRepository, LicitacionRepository>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddDbContext<LicitacionesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Licitaciones")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
