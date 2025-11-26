using receivables.Domain.DTOs.Cart;
using receivables.Domain.DTOs.Checkout;

namespace receivables.Domain.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> AddInvoiceToCartAsync(Guid companyId, Guid invoiceId);
        Task<CartDto> RemoveInvoiceFromCartAsync(Guid companyId, Guid invoiceId);
        Task<CartDto> GetCartAsync(Guid companyId);
        Task<CheckoutDto> GetCartCheckoutAsync(Guid companyId);
    }
}
