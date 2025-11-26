using receivables.Infrastructure.Entities;

namespace receivables.Infrastructure.Repositories;

public interface ICartRepository
{
    Task<IEnumerable<Invoice>> GetCartItemsByCompanyAsync(Guid companyId);
    Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId);
    Task AddInvoiceToCartAsync(Guid invoiceId);
    Task RemoveInvoiceFromCartAsync(Guid invoiceId);
    Task<bool> IsInvoiceInCartAsync(Guid invoiceId);
    Task<decimal> GetCartTotalAsync(Guid companyId);
    Task SaveChangesAsync();
}
