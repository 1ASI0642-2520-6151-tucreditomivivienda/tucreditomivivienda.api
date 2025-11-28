namespace TuCreditoMiVivienda.Api.LoanContext.Domain;

public class LoanSimulation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Datos del request
    public decimal Principal { get; set; }
    public string Currency { get; set; } = "PEN";
    public string RateType { get; set; } = "efectiva";
    public decimal RateValue { get; set; }
    public string Capitalization { get; set; } = "mensual";
    public int TermMonths { get; set; }
    public string GraceType { get; set; } = "sin";
    public int GraceMonths { get; set; } = 0;
    
    // Resumen del resultado
    public decimal MonthlyRate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalInterest { get; set; }
    
    // Opcional: relacionar con un cliente
    public Guid? ClientId { get; set; }
    
    // Opcional: relacionar con una propiedad
    public Guid? PropertyId { get; set; }
}

