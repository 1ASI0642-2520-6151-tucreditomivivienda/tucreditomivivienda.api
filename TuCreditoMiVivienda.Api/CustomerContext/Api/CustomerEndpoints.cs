using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TuCreditoMiVivienda.Api.CustomerContext.Data;
using TuCreditoMiVivienda.Api.CustomerContext.Domain;

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
        });

        // ---------- CLIENTES ----------
        app.MapGet("/api/clients", (CustomersDb db) =>
            Results.Ok(db.Clients));

        app.MapGet("/api/clients/{id:guid}", (Guid id, CustomersDb db) =>
        {
            var client = db.Clients.FirstOrDefault(c => c.Id == id);
            return client is null ? Results.NotFound() : Results.Ok(client);
        });

        app.MapPost("/api/clients", (CustomersDb db, [FromBody] Client client) =>
        {
            if (client.Id == Guid.Empty)
                client.Id = Guid.NewGuid();

            db.Clients.Add(client);
            return Results.Created($"/api/clients/{client.Id}", client);
        });

        app.MapPut("/api/clients/{id:guid}", (Guid id, CustomersDb db, [FromBody] Client updated) =>
        {
            var client = db.Clients.FirstOrDefault(c => c.Id == id);
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

            return Results.Ok(client);
        });

        app.MapDelete("/api/clients/{id:guid}", (Guid id, CustomersDb db) =>
        {
            var client = db.Clients.FirstOrDefault(c => c.Id == id);
            if (client is null) return Results.NotFound();

            db.Clients.Remove(client);
            return Results.NoContent();
        });

        return app;
    }
}
