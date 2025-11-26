using Microsoft.EntityFrameworkCore;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;
using Receivables.Infrastructure.Persistence;

namespace Receivables.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _ctx;

    public CartRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Invoice>> GetCartItemsByCompanyAsync(Guid companyId) =>
        await _ctx.Invoices
            .Where(i => i.CompanyId == companyId && i.InCart)
            .OrderBy(i => i.Number)
            .ToListAsync();

    public Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId) =>
        _ctx.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId)!;

    public async Task AddInvoiceToCartAsync(Guid invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if (invoice != null)
        {
            invoice.AddToCart();
            _ctx.Invoices.Update(invoice);
        }
    }

    public async Task RemoveInvoiceFromCartAsync(Guid invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if (invoice != null)
        {
            invoice.RemoveFromCart();
            _ctx.Invoices.Update(invoice);
        }
    }

    public async Task<bool> IsInvoiceInCartAsync(Guid invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        return invoice?.InCart ?? false;
    }

    public async Task<decimal> GetCartTotalAsync(Guid companyId) =>
        await _ctx.Invoices
            .Where(i => i.CompanyId == companyId && i.InCart)
            .SumAsync(i => i.Amount);

    public Task SaveChangesAsync() => _ctx.SaveChangesAsync();
}
