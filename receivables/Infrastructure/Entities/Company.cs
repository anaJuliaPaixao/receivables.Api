using Receivables.Domain.Enums;
using Receivables.Domain.Services;

namespace receivables.Infrastructure.Entities;

public class Company
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Cnpj { get; private set; }
    public string Name { get; private set; }
    public decimal MonthlyRevenue { get; private set; }
    public CompanySegment Segment { get; private set; }
    public decimal CreditLimit { get; private set; }

    private readonly List<Invoice> _invoices = new();
    public IReadOnlyCollection<Invoice> Invoices => _invoices;

    private Company() { }

    public Company(string cnpj, string name, decimal monthlyRevenue, CompanySegment segment, ICreditLimitCalculator limitCalculator)
    {
        Cnpj = cnpj;
        Name = name;
        MonthlyRevenue = monthlyRevenue;
        Segment = segment;
        RecalculateCreditLimit(limitCalculator);
    }

    public void RecalculateCreditLimit(ICreditLimitCalculator calculator)
    {
        CreditLimit = calculator.Calculate(MonthlyRevenue, Segment);
    }
}
