namespace receivables.Domain.DTOs.Cart
{
    public class CartInvoiceDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}
