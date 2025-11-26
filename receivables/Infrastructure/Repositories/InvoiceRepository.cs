using Microsoft.EntityFrameworkCore;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;
using Receivables.Infrastructure.Persistence;

namespace Receivables.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _ctx;
    public InvoiceRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Invoice invoice) => await _ctx.Invoices.AddAsync(invoice);

    public Task<Invoice?> GetByIdAsync(Guid id) => _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == id)!;

    public Task<Invoice?> GetByNumberAndCompanyAsync(string number, Guid companyId) => 
        _ctx.Invoices.FirstOrDefaultAsync(i => i.Number == number && i.CompanyId == companyId)!;

    public async Task<IEnumerable<Invoice>> GetByCompanyAsync(Guid companyId) => 
        await _ctx.Invoices.Where(i => i.CompanyId == companyId).ToListAsync();

    public async Task<IEnumerable<Invoice>> GetInCartByCompanyAsync(Guid companyId) => 
        await _ctx.Invoices.Where(i => i.CompanyId == companyId && i.InCart).ToListAsync();

    public void Update(Invoice invoice) => _ctx.Invoices.Update(invoice);

    public Task SaveChangesAsync() => _ctx.SaveChangesAsync();
}
