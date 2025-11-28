using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TuCreditoMiVivienda.Api.LoanContext.Data;
using TuCreditoMiVivienda.Api.LoanContext.Domain;
using TuCreditoMiVivienda.Api.LoanContext.Services;

namespace TuCreditoMiVivienda.Api.LoanContext.Api;

public static class LoanEndpoints
{
    public static IEndpointRouteBuilder MapLoanEndpoints(this IEndpointRouteBuilder app)
    {
        // ---------- PROPIEDADES ----------
        app.MapGet("/api/properties", (RealEstateDb db) =>
            Results.Ok(db.Properties));

        app.MapGet("/api/properties/{id:guid}", (Guid id, RealEstateDb db) =>
        {
            var prop = db.Properties.FirstOrDefault(p => p.Id == id);
            return prop is null ? Results.NotFound() : Results.Ok(prop);
        });

        app.MapPost("/api/properties", (RealEstateDb db, [FromBody] Property property) =>
        {
            if (property.Id == Guid.Empty)
                property.Id = Guid.NewGuid();

            db.Properties.Add(property);
            return Results.Created($"/api/properties/{property.Id}", property);
        });

        app.MapPut("/api/properties/{id:guid}", (Guid id, RealEstateDb db, [FromBody] Property updated) =>
        {
            var prop = db.Properties.FirstOrDefault(p => p.Id == id);
            if (prop is null) return Results.NotFound();

            prop.Proyecto = updated.Proyecto;
            prop.Torre = updated.Torre;
            prop.Numero = updated.Numero;
            prop.Ciudad = updated.Ciudad;
            prop.TipoUnidad = updated.TipoUnidad;
            prop.AreaM2 = updated.AreaM2;
            prop.Dormitorios = updated.Dormitorios;
            prop.Banos = updated.Banos;
            prop.PrecioVenta = updated.PrecioVenta;
            prop.Estado = updated.Estado;

            return Results.Ok(prop);
        });

        app.MapDelete("/api/properties/{id:guid}", (Guid id, RealEstateDb db) =>
        {
            var prop = db.Properties.FirstOrDefault(p => p.Id == id);
            if (prop is null) return Results.NotFound();

            db.Properties.Remove(prop);
            return Results.NoContent();
        });

        // ---------- SIMULACIÓN DE CRÉDITO ----------
        app.MapPost("/api/loans/simulate", (
            [FromBody] LoanSimulationRequest request) =>
        {
            try
            {
                var result = LoanCalculator.Simulate(request);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        return app;
    }
}
