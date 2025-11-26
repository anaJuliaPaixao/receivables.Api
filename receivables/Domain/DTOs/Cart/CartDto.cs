namespace receivables.Domain.DTOs.Cart
{
    public class CartDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public List<CartInvoiceDto> Invoices { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal TotalGross { get; set; }
    }
}
