using System.ComponentModel.DataAnnotations;

namespace receivables.Domain.DTOs.Cart
{
    public class CartItemRequest
    {
        [Required(ErrorMessage = "O ID da nota fiscal é obrigatório")]
        public Guid InvoiceId { get; set; }
    }
}
