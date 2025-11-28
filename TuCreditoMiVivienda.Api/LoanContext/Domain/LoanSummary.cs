namespace TuCreditoMiVivienda.Api.LoanContext.Domain;

public class LoanSummary
{
    public decimal Principal { get; set; }
    public int TermMonths { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalInterest { get; set; }
}