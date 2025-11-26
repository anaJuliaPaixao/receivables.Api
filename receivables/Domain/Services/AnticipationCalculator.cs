namespace Receivables.Domain.Services;

public class AnticipationCalculator
{
    private const decimal MonthlyRate = 0.0465m;

    public decimal CalculateNetValue(decimal grossValue, DateTime dueDate, DateTime nowUtc)
    {
        var days = (dueDate.Date - nowUtc.Date).Days; 
        if (days <= 0) return 0m;
        var exponent = days / 30.0m;
        var discount = grossValue / Pow(1 + MonthlyRate, exponent);
        var net = grossValue - discount;
        return decimal.Round(net, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal Pow(decimal baseValue, decimal exp)
    {
        var result = Math.Pow((double)baseValue, (double)exp);
        return (decimal)result;
    }
}
