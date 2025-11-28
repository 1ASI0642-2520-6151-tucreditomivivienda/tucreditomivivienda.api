using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using TuCreditoMiVivienda.Api.Data;
using TuCreditoMiVivienda.Api.LoanContext.Domain;
using TuCreditoMiVivienda.Api.LoanContext.Services;

namespace TuCreditoMiVivienda.Api.LoanContext.Api;

public static class LoanEndpoints
{
    public static IEndpointRouteBuilder MapLoanEndpoints(this IEndpointRouteBuilder app)
    {
        // ---------- PROPIEDADES ----------
        app.MapGet("/api/properties", async (AppDbContext db) =>
            Results.Ok(await db.Properties.ToListAsync()))
            .WithTags("Properties");

        app.MapGet("/api/properties/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var prop = await db.Properties.FirstOrDefaultAsync(p => p.Id == id);
            return prop is null ? Results.NotFound() : Results.Ok(prop);
        })
        .WithTags("Properties");

        app.MapPost("/api/properties", async (AppDbContext db, [FromBody] Property property) =>
        {
            if (property.Id == Guid.Empty)
                property.Id = Guid.NewGuid();

            db.Properties.Add(property);
            await db.SaveChangesAsync();
            return Results.Created($"/api/properties/{property.Id}", property);
        })
        .WithTags("Properties");

        app.MapPut("/api/properties/{id:guid}", async (Guid id, AppDbContext db, [FromBody] Property updated) =>
        {
            var prop = await db.Properties.FirstOrDefaultAsync(p => p.Id == id);
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

            await db.SaveChangesAsync();
            return Results.Ok(prop);
        })
        .WithTags("Properties");

        app.MapDelete("/api/properties/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var prop = await db.Properties.FirstOrDefaultAsync(p => p.Id == id);
            if (prop is null) return Results.NotFound();

            db.Properties.Remove(prop);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithTags("Properties");

        // ---------- SIMULACIÓN DE CRÉDITO ----------
        app.MapPost("/api/loans/simulate", async (
            AppDbContext db,
            [FromBody] LoanSimulationRequest request) =>
        {
            try
            {
                var result = LoanCalculator.Simulate(request);
                
                // Guardar la simulación en la base de datos
                var simulation = new LoanSimulation
                {
                    Principal = request.Principal,
                    Currency = request.Config.Currency,
                    RateType = request.Config.RateType,
                    RateValue = request.Config.RateValue,
                    Capitalization = request.Config.Capitalization,
                    TermMonths = request.Config.TermMonths,
                    GraceType = request.Config.GraceType,
                    GraceMonths = request.Config.GraceMonths,
                    MonthlyRate = result.Summary.MonthlyRate,
                    MonthlyPayment = result.Summary.MonthlyPayment,
                    TotalPaid = result.Summary.TotalPaid,
                    TotalInterest = result.Summary.TotalInterest,
                    ClientId = request.ClientId,
                    PropertyId = request.PropertyId
                };
                
                db.LoanSimulations.Add(simulation);
                await db.SaveChangesAsync();
                
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithTags("Loans");
        
        // ---------- HISTORIAL DE SIMULACIONES ----------
        app.MapGet("/api/loans/simulations", async (AppDbContext db) =>
            Results.Ok(await db.LoanSimulations
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync()))
            .WithTags("Simulations");
        
        app.MapGet("/api/loans/simulations/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var simulation = await db.LoanSimulations.FirstOrDefaultAsync(s => s.Id == id);
            return simulation is null ? Results.NotFound() : Results.Ok(simulation);
        })
        .WithTags("Simulations");
        
        app.MapDelete("/api/loans/simulations/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var simulation = await db.LoanSimulations.FirstOrDefaultAsync(s => s.Id == id);
            if (simulation is null) return Results.NotFound();
            
            db.LoanSimulations.Remove(simulation);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithTags("Simulations");

        return app;
    }
}
