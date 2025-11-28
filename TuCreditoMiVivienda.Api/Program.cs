using Microsoft.EntityFrameworkCore;
using TuCreditoMiVivienda.Api.CustomerContext.Api;
using TuCreditoMiVivienda.Api.CustomerContext.Domain;
using TuCreditoMiVivienda.Api.Data;
using TuCreditoMiVivienda.Api.LoanContext.Api;
using TuCreditoMiVivienda.Api.LoanContext.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: permitir llamadas desde el frontend
var corsPolicy = "AllowFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy
            .AllowAnyOrigin()   // para pruebas, todo permitido
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// SQLite Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Asegurar que la base de datos esté creada y con datos iniciales
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    
    // Seed data si la base de datos está vacía
    if (!dbContext.Clients.Any())
    {
        dbContext.Clients.Add(new Client
        {
            Nombres = "Juan",
            Apellidos = "Pérez",
            Documento = "12345678",
            EstadoCivil = "Soltero",
            IngresosMensuales = 3500,
            Dependientes = 0,
            ActividadLaboral = "Dependiente",
            Telefono = "999999999",
            Email = "juan@example.com",
            UnidadInteres = "Proyecto A - Dpto 301"
        });
        dbContext.SaveChanges();
    }
    
    if (!dbContext.Properties.Any())
    {
        dbContext.Properties.Add(new Property
        {
            Proyecto = "Residencial Callao",
            Torre = "A",
            Numero = "301",
            Ciudad = "Lima",
            TipoUnidad = "Departamento",
            AreaM2 = 75,
            Dormitorios = 3,
            Banos = 2,
            PrecioVenta = 250_000,
            Estado = "disponible"
        });
        dbContext.SaveChanges();
    }
}

// Swagger disponible en desarrollo y producción (útil para testing en Render)
app.UseSwagger();
app.UseSwaggerUI();

// En Render, no usar HTTPS redirection (Render maneja HTTPS automáticamente)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// usar CORS antes de mapear endpoints
app.UseCors(corsPolicy);

// BC 1: Clientes (login + clientes)
app.MapCustomerEndpoints();

// BC 2: Créditos/Vivienda (propiedades + simulación)
app.MapLoanEndpoints();

// Render usa la variable de entorno PORT, si no existe usa el puerto por defecto
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Run($"http://0.0.0.0:{port}");
}
else
{
    app.Run();
}