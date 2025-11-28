using TuCreditoMiVivienda.Api.CustomerContext.Api;
using TuCreditoMiVivienda.Api.CustomerContext.Data;
using TuCreditoMiVivienda.Api.LoanContext.Api;
using TuCreditoMiVivienda.Api.LoanContext.Data;

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

// "Bases de datos" por bounded context
builder.Services.AddSingleton<CustomersDb>();
builder.Services.AddSingleton<RealEstateDb>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// usar CORS antes de mapear endpoints
app.UseCors(corsPolicy);

// BC 1: Clientes (login + clientes)
app.MapCustomerEndpoints();

// BC 2: Créditos/Vivienda (propiedades + simulación)
app.MapLoanEndpoints();

app.Run();