namespace receivables.Domain.DTOs.Checkout;

public class CheckoutDto
{
    public string Company { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public List<CheckoutInvoiceItemDto> Invoices { get; set; } = new();
    public decimal TotalNet { get; set; }
    public decimal TotalGross { get; set; }
}