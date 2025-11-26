namespace receivables.Domain.DTOs.Checkout;

public class InvoiceCalculationDto
{
    public string Number { get; set; } = string.Empty;
    public decimal GrossValue { get; set; }
    public decimal NetValue { get; set; }
}
