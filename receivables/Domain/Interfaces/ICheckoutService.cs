using receivables.Domain.DTOs.Checkout;

namespace receivables.Domain.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutDto> CalculateCheckoutAsync(Guid companyId);
        Task<InvoiceCalculationDto> CalculateCheckoutByInvoiceIdAsync(Guid invoiceId);
    }
}
