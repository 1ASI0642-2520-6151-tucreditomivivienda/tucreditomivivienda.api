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

        // Validar y ajustar meses de gracia
        var graceMonths = cfg.GraceMonths;
        if (graceMonths < 0) graceMonths = 0;
        if (graceMonths > termMonths) graceMonths = termMonths;
        
        var graceType = cfg.GraceType?.ToLower() ?? "sin";
        if (graceType != "sin" && graceType != "total" && graceType != "parcial")
            graceType = "sin";
        
        // Si no hay gracia o es "sin", no aplicar gracia
        if (graceMonths == 0 || graceType == "sin")
            graceMonths = 0;

        var monthlyRate = GetMonthlyRate(cfg);
        var schedule = new List<LoanPayment>();
        var balance = principal;
        decimal totalPaid = 0;
        decimal totalInterest = 0;
        decimal frenchPayment = 0;
        var remainingMonths = termMonths - graceMonths;

        // Calcular cuota del tramo francés (si hay meses restantes)
        if (remainingMonths > 0 && monthlyRate > 0)
        {
            var r = monthlyRate;
            var n = remainingMonths;
            var pow = (decimal)Math.Pow(1 + (double)r, n);
            frenchPayment = balance * r * pow / (pow - 1);
        }
        else if (remainingMonths > 0)
        {
            frenchPayment = balance / remainingMonths;
        }

        // Procesar meses de gracia
        for (int month = 1; month <= graceMonths; month++)
        {
            var interest = Math.Round(balance * monthlyRate, 2);
            decimal payment = 0;
            decimal principalPaid = 0;
            decimal newBalance = balance;

            if (graceType == "total")
            {
                // Gracia total: no se paga nada, intereses se capitalizan
                payment = 0;
                principalPaid = 0;
                newBalance = Math.Round(balance + interest, 2);
                totalInterest += interest; // Los intereses se capitalizan pero se cuentan en total
            }
            else if (graceType == "parcial")
            {
                // Gracia parcial: se pagan solo intereses
                payment = interest;
                principalPaid = 0;
                newBalance = balance; // El saldo no cambia
                totalPaid += payment;
                totalInterest += interest;
            }

            schedule.Add(new LoanPayment
            {
                Month = month,
                Payment = Math.Round(payment, 2),
                Interest = interest,
                PrincipalPaid = principalPaid,
                Balance = newBalance
            });

            balance = newBalance;
        }

        // Actualizar cuota francesa si hubo capitalización en gracia total
        if (graceMonths > 0 && graceType == "total" && remainingMonths > 0 && monthlyRate > 0)
        {
            var r = monthlyRate;
            var n = remainingMonths;
            var pow = (decimal)Math.Pow(1 + (double)r, n);
            frenchPayment = balance * r * pow / (pow - 1);
        }

        // Procesar tramo francés
        for (int month = graceMonths + 1; month <= termMonths; month++)
        {
            var interest = Math.Round(balance * monthlyRate, 2);
            var principalPaid = Math.Round(frenchPayment - interest, 2);
            var payment = frenchPayment;

            // Ajuste en el último mes
            if (month == termMonths)
            {
                principalPaid = balance;
                payment = Math.Round(principalPaid + interest, 2);
            }

            balance = Math.Round(balance - principalPaid, 2);
            totalPaid += payment;
            totalInterest += interest;

            schedule.Add(new LoanPayment
            {
                Month = month,
                Payment = Math.Round(payment, 2),
                Interest = interest,
                PrincipalPaid = principalPaid,
                Balance = balance
            });
        }

        // Calcular cuota promedio (solo del tramo francés, excluyendo gracia)
        var averagePayment = remainingMonths > 0 ? frenchPayment : 0;

        var summary = new LoanSummary
        {
            Principal = principal,
            TermMonths = termMonths,
            MonthlyRate = Math.Round(monthlyRate * 100, 4),
            MonthlyPayment = Math.Round(averagePayment, 2),
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
