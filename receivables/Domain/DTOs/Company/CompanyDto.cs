using Receivables.Domain.Enums;

namespace receivables.Domain.DTOs.Company;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyRevenue { get; set; }
    public CompanySegment Segment { get; set; }
    public decimal CreditLimit { get; set; }
}