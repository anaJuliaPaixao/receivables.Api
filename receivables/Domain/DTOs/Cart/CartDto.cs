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

    public class CartInvoiceDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}
