using System.Text.Encodings.Web;
using System.Text.Unicode;

using Licitaciones.Application.Aprobaciones;
using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Application.Licitaciones.Eliminar;
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

using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(HtmlEncoder.Create(
    UnicodeRanges.BasicLatin,
    UnicodeRanges.Latin1Supplement,
    UnicodeRanges.CurrencySymbols));
builder.Services.AddOpenApi();
builder.Services.AddScoped<CrearProveedorService>();
builder.Services.AddScoped<ConsultarProveedorService>();
builder.Services.AddScoped<EditarProveedorService>();
builder.Services.AddScoped<DarBajaProveedorService>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorConsultaRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorBajaRepository, ProveedorRepository>();
builder.Services.AddScoped<CrearLicitacionService>();
builder.Services.AddScoped<ConsultarLicitacionService>();
builder.Services.AddScoped<PublicarLicitacionService>();
builder.Services.AddScoped<EditarLicitacionService>();
builder.Services.AddScoped<EliminarLicitacionService>();
builder.Services.AddScoped<ILicitacionRepository, LicitacionRepository>();
builder.Services.AddScoped<ILicitacionConsultaRepository, LicitacionConsultaRepository>();
builder.Services.AddScoped<ILicitacionBajaRepository, LicitacionRepository>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<IOfertaConsultaRepository, OfertaRepository>();
builder.Services.AddScoped<IEditarOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<IEliminarOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<ConsultarOfertaService>();
builder.Services.AddScoped<CrearOfertaService>();
builder.Services.AddScoped<EditarOfertaService>();
builder.Services.AddScoped<EliminarOfertaService>();
builder.Services.AddScoped<AdministrarNivelesAprobacionService>();
builder.Services.AddScoped<INivelAprobacionRepository, NivelAprobacionRepository>();
builder.Services.AddScoped<AdministrarTipoCambioService>();
builder.Services.AddScoped<ITipoCambioRepository, TipoCambioRepository>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddDbContext<LicitacionesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Licitaciones")));

var app = builder.Build();

if (builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<LicitacionesDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "Licitaciones API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

