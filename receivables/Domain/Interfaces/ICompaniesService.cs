using receivables.Domain.DTOs.Company;

namespace receivables.Domain.Interfaces
{
    public interface ICompaniesService
    {
        Task<CompanyDto> CreateAsync(CompanyRequest companyRequest);
    }
}
