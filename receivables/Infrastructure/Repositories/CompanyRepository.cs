using Microsoft.EntityFrameworkCore;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;
using Receivables.Infrastructure.Persistence;

namespace Receivables.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _ctx;
    public CompanyRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Company company) => await _ctx.Companies.AddAsync(company);

    public Task<Company?> GetByIdAsync(Guid id) => _ctx.Companies.FirstOrDefaultAsync(c => c.Id == id)!;

    public Task<Company?> GetByCnpjAsync(string cnpj) => _ctx.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj)!;

    public Task SaveChangesAsync() => _ctx.SaveChangesAsync();
}
