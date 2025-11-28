namespace TuCreditoMiVivienda.Api.LoanContext.Domain;

public class LoanPayment
{
    public int Month { get; set; }
    public decimal Payment { get; set; }
    public decimal Interest { get; set; }
    public decimal PrincipalPaid { get; set; }
    public decimal Balance { get; set; }
}