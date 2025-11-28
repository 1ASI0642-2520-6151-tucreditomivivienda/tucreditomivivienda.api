namespace TuCreditoMiVivienda.Api.LoanContext.Domain;

public class LoanSimulationRequest
{
    public decimal Principal { get; set; }
    public LoanConfig Config { get; set; } = new();
    public Guid? ClientId { get; set; }
    public Guid? PropertyId { get; set; }
}