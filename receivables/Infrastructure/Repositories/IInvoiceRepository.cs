using receivables.Infrastructure.Entities;

namespace receivables.Infrastructure.Repositories;

public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice);
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<Invoice?> GetByNumberAndCompanyAsync(string number, Guid companyId);
    Task<IEnumerable<Invoice>> GetByCompanyAsync(Guid companyId);
    Task<IEnumerable<Invoice>> GetInCartByCompanyAsync(Guid companyId);
    void Update(Invoice invoice);
    Task SaveChangesAsync();
}