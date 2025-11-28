using TuCreditoMiVivienda.Api.LoanContext.Domain;

namespace TuCreditoMiVivienda.Api.LoanContext.Services;

public static class LoanCalculator
{
    public static LoanSimulationResult Simulate(LoanSimulationRequest request)
    {
        var principal = request.Principal;
        var cfg = request.Config;
        var termMonths = cfg.TermMonths;

        if (principal <= 0 || termMonths <= 0)
            throw new ArgumentException("Monto y plazo deben ser mayores a cero.");

        var monthlyRate = GetMonthlyRate(cfg);

        decimal monthlyPayment;
        if (monthlyRate > 0)
        {
            var r = monthlyRate;
            var n = termMonths;
            var pow = (decimal)Math.Pow(1 + (double)r, n);
            monthlyPayment = principal * r * pow / (pow - 1);
        }
        else
        {
            monthlyPayment = principal / termMonths;
        }

        var schedule = new List<LoanPayment>();
        var balance = principal;
        decimal totalPaid = 0;
        decimal totalInterest = 0;

        for (int month = 1; month <= termMonths; month++)
        {
            var interest = Math.Round(balance * monthlyRate, 2);
            var principalPaid = Math.Round(monthlyPayment - interest, 2);

            if (month == termMonths)
            {
                principalPaid = balance;
                monthlyPayment = principalPaid + interest;
            }

            balance = Math.Round(balance - principalPaid, 2);
            totalPaid += monthlyPayment;
            totalInterest += interest;

            schedule.Add(new LoanPayment
            {
                Month = month,
                Payment = Math.Round(monthlyPayment, 2),
                Interest = interest,
                PrincipalPaid = principalPaid,
                Balance = balance
            });
        }

        var summary = new LoanSummary
        {
            Principal = principal,
            TermMonths = termMonths,
            MonthlyRate = Math.Round(monthlyRate * 100, 4),
            MonthlyPayment = Math.Round(monthlyPayment, 2),
            TotalPaid = Math.Round(totalPaid, 2),
            TotalInterest = Math.Round(totalInterest, 2)
        };

        return new LoanSimulationResult
        {
            Schedule = schedule,
            Summary = summary
        };
    }

    private static decimal GetMonthlyRate(LoanConfig config)
    {
        var rate = config.RateValue / 100m;
        if (rate <= 0) return 0;

        if (config.RateType == "efectiva")
        {
            var monthly = Math.Pow(1 + (double)rate, 1.0 / 12.0) - 1.0;
            return (decimal)monthly;
        }

        var m = GetCapitalizationsPerYear(config.Capitalization);
        var tea = Math.Pow(1 + (double)rate / m, m) - 1.0;
        var monthlyFromTea = Math.Pow(1 + tea, 1.0 / 12.0) - 1.0;
        return (decimal)monthlyFromTea;
    }

    private static int GetCapitalizationsPerYear(string cap)
    {
        return cap switch
        {
            "mensual" => 12,
            "bimestral" => 6,
            "trimestral" => 4,
            "semestral" => 2,
            _ => 12
        };
    }
}
