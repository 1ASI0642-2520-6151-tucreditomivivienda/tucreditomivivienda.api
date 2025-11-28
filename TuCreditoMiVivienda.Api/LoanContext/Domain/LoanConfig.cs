namespace TuCreditoMiVivienda.Api.LoanContext.Domain;

public class LoanConfig
{
    public string Currency { get; set; } = "PEN";
    public string RateType { get; set; } = "efectiva";   // efectiva | nominal
    public decimal RateValue { get; set; } = 12m;        // %
    public string Capitalization { get; set; } = "mensual";
    public int TermMonths { get; set; } = 240;
}