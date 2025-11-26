using receivables.Infrastructure.Entities;

namespace receivables.Infrastructure.Repositories;

public interface ICompanyRepository
{
    Task AddAsync(Company company);
    Task<Company?> GetByIdAsync(Guid id);
    Task<Company?> GetByCnpjAsync(string cnpj);
    Task SaveChangesAsync();
}