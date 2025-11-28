namespace TuCreditoMiVivienda.Api.LoanContext.Domain;

public class LoanSimulationResult
{
    public List<LoanPayment> Schedule { get; set; } = new();
    public LoanSummary Summary { get; set; } = new();
}