using Receivables.Domain.Enums;

namespace Receivables.Domain.Services;

public interface ICreditLimitCalculator
{
    decimal Calculate(decimal monthlyRevenue, CompanySegment segment);
}

public sealed class CreditLimitCalculator : ICreditLimitCalculator
{
    // Faixas de faturamento
    private const decimal MinimumSupportedRevenue = 10_000m;
    private const decimal Bracket1Max = 50_000m;
    private const decimal Bracket2Max = 100_000m;

    // Percentuais (Services / Products)
    private const decimal Bracket1Rate = 0.50m; // Iguais para ambos os segmentos
    private const decimal Bracket2RateServices = 0.55m;
    private const decimal Bracket2RateProducts = 0.60m;
    private const decimal Bracket3RateServices = 0.60m;
    private const decimal Bracket3RateProducts = 0.65m;

    public decimal Calculate(decimal monthlyRevenue, CompanySegment segment)
    {
        if (monthlyRevenue < MinimumSupportedRevenue)
            return 0m; // Fora da faixa suportada (regra de negócio atual)

        return (monthlyRevenue, segment) switch
        {
            (<= Bracket1Max, _) => monthlyRevenue * Bracket1Rate,
            (<= Bracket2Max, CompanySegment.Services) => monthlyRevenue * Bracket2RateServices,
            (<= Bracket2Max, CompanySegment.Products) => monthlyRevenue * Bracket2RateProducts,
            (> Bracket2Max, CompanySegment.Services) => monthlyRevenue * Bracket3RateServices,
            (> Bracket2Max, CompanySegment.Products) => monthlyRevenue * Bracket3RateProducts,
            _ => 0m
        };
    }
}
