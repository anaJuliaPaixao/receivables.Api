using receivables.Domain.Interfaces;
using receivables.Infrastructure.Repositories;
using receivables.Infrastructure.Entities;
using Receivables.Domain.Services;
using receivables.Domain.DTOs.Company;

namespace receivables.Domain.Services
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICreditLimitCalculator _creditLimitCalculator;

        public CompaniesService(
            ICompanyRepository companyRepository,
            ICreditLimitCalculator creditLimitCalculator)
        {
            _companyRepository = companyRepository;
            _creditLimitCalculator = creditLimitCalculator;
        }

        public async Task<CompanyDto> CreateAsync(CompanyRequest companyRequest)
        {
            var existingCompany = await _companyRepository.GetByCnpjAsync(companyRequest.Cnpj);

            if (existingCompany != null)
            {
                throw new InvalidOperationException($"Já existe uma empresa cadastrada com o CNPJ {companyRequest.Cnpj}");
            }
            ValidateMinimumRevenue(companyRequest.MonthlyRevenue);

          
            var company = CreateCompanyEntity(companyRequest);

            await PersistCompany(company);

            return new CompanyDto
            {
                Id = company.Id,
                Cnpj = company.Cnpj,
                Name = company.Name,
                MonthlyRevenue = company.MonthlyRevenue,
                Segment = company.Segment,
                CreditLimit = company.CreditLimit
            };
        }

        private static void ValidateMinimumRevenue(decimal monthlyRevenue)
        {
            const decimal minimumRevenue = 10_000m;

            if (monthlyRevenue < minimumRevenue)
            {
                throw new InvalidOperationException(
                    $"O faturamento mensal deve ser no mínimo R$ {minimumRevenue:N2}. Valor atual: R$ {monthlyRevenue:N2}");
            }
        }

        private Company CreateCompanyEntity(CompanyRequest request)
        {
            return new Company(
                cnpj: request.Cnpj,
                name: request.Name,
                monthlyRevenue: request.MonthlyRevenue,
                segment: request.Segment,
                limitCalculator: _creditLimitCalculator
            );
        }

        private async Task PersistCompany(Company company)
        {
            await _companyRepository.AddAsync(company);
            await _companyRepository.SaveChangesAsync();
        }
    }
}
