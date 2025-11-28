using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using TuCreditoMiVivienda.Api.CustomerContext.Domain;
using TuCreditoMiVivienda.Api.Data;

namespace TuCreditoMiVivienda.Api.CustomerContext.Api;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        // ---------- AUTH (login demo) ----------
        app.MapPost("/api/auth/login", (
            [FromBody] LoginRequest request) =>
        {
            const string demoEmail = "asesor@demo.com";
            const string demoPassword = "admin123";

            if (request.Email == demoEmail && request.Password == demoPassword)
            {
                var user = new User
                {
                    Nombre = "Asesor Demo",
                    Email = demoEmail,
                    Rol = "asesor"
                };

                var token = Guid.NewGuid().ToString();

                return Results.Ok(new LoginResponse(token, user));
            }

            return Results.BadRequest(new { message = "Credenciales incorrectas" });
        })
        .WithTags("Authentication");

        // ---------- CLIENTES ----------
        app.MapGet("/api/clients", async (AppDbContext db) =>
            Results.Ok(await db.Clients.ToListAsync()))
            .WithTags("Clients");

        app.MapGet("/api/clients/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == id);
            return client is null ? Results.NotFound() : Results.Ok(client);
        })
        .WithTags("Clients");

        app.MapPost("/api/clients", async (AppDbContext db, [FromBody] Client client) =>
        {
            if (client.Id == Guid.Empty)
                client.Id = Guid.NewGuid();

            db.Clients.Add(client);
            await db.SaveChangesAsync();
            return Results.Created($"/api/clients/{client.Id}", client);
        })
        .WithTags("Clients");

        app.MapPut("/api/clients/{id:guid}", async (Guid id, AppDbContext db, [FromBody] Client updated) =>
        {
            var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client is null) return Results.NotFound();

            client.Nombres = updated.Nombres;
            client.Apellidos = updated.Apellidos;
            client.Documento = updated.Documento;
            client.EstadoCivil = updated.EstadoCivil;
            client.IngresosMensuales = updated.IngresosMensuales;
            client.Dependientes = updated.Dependientes;
            client.ActividadLaboral = updated.ActividadLaboral;
            client.Telefono = updated.Telefono;
            client.Email = updated.Email;
            client.UnidadInteres = updated.UnidadInteres;

            await db.SaveChangesAsync();
            return Results.Ok(client);
        })
        .WithTags("Clients");

        app.MapDelete("/api/clients/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client is null) return Results.NotFound();

            db.Clients.Remove(client);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithTags("Clients");

        return app;
    }
}
