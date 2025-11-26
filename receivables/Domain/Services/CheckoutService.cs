using receivables.Domain.DTOs.Checkout;
using receivables.Domain.Interfaces;
using receivables.Infrastructure.Repositories;
using Receivables.Domain.Services;

namespace receivables.Domain.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly AnticipationCalculator _anticipationCalculator;

        public CheckoutService(
            ICompanyRepository companyRepository,
            IInvoiceRepository invoiceRepository,
            AnticipationCalculator anticipationCalculator)
        {
            _companyRepository = companyRepository;
            _invoiceRepository = invoiceRepository;
            _anticipationCalculator = anticipationCalculator;
        }

        public async Task<CheckoutDto> CalculateCheckoutAsync(Guid companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                throw new InvalidOperationException($"Empresa com ID {companyId} não foi encontrada");
            }

            var invoicesInCart = await _invoiceRepository.GetInCartByCompanyAsync(companyId);
            var invoicesList = invoicesInCart.ToList();

            if (!invoicesList.Any())
            {
                throw new InvalidOperationException("Não há notas fiscais no carrinho");
            }

            var now = DateTime.UtcNow;
            var checkoutInvoices = new List<CheckoutInvoiceItemDto>();
            decimal totalGross = 0m;
            decimal totalNet = 0m;

            foreach (var invoice in invoicesList.OrderBy(i => i.Number))
            {
                var netValue = _anticipationCalculator.CalculateNetValue(invoice.Amount, invoice.DueDate, now);
                
                checkoutInvoices.Add(new CheckoutInvoiceItemDto
                {
                    Number = invoice.Number,
                    GrossValue = invoice.Amount,
                    NetValue = netValue
                });

                totalGross += invoice.Amount;
                totalNet += netValue;
            }

            if (totalGross > company.CreditLimit)
            {
                throw new InvalidOperationException($"O valor bruto total (R$ {totalGross:N2}) excede o limite de crédito (R$ {company.CreditLimit:N2})");
            }

            return new CheckoutDto
            {
                Company = company.Name,
                Cnpj = company.Cnpj,
                CreditLimit = company.CreditLimit,
                Invoices = checkoutInvoices,
                TotalNet = totalNet,
                TotalGross = totalGross
            };
        }

        public async Task<InvoiceCalculationDto> CalculateCheckoutByInvoiceIdAsync(Guid invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new InvalidOperationException($"Nota fiscal com ID {invoiceId} não foi encontrada");
            }

            var now = DateTime.UtcNow;
            var netValue = _anticipationCalculator.CalculateNetValue(invoice.Amount, invoice.DueDate, now);

            return new InvoiceCalculationDto
            {
                Number = invoice.Number,
                GrossValue = invoice.Amount,
                NetValue = netValue
            };
        }
    }
}
