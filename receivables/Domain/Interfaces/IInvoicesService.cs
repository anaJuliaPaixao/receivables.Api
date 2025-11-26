using receivables.Domain.DTOs.Invoice;

namespace receivables.Domain.Interfaces
{
    public interface IInvoicesService
    {
        Task<InvoiceDto> CreateAsync(InvoiceRequest invoiceRequest);
        Task<InvoiceDto> UpdateAsync(Guid id, UpdateInvoiceRequest updateRequest);
    }
}
