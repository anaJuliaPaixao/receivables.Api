using receivables.Domain.DTOs.Cart;
using receivables.Domain.DTOs.Checkout;
using receivables.Domain.Interfaces;
using receivables.Infrastructure.Repositories;
using Receivables.Domain.Services;

namespace receivables.Domain.Services
{
    public class CartService : ICartService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICheckoutService _checkoutService;

        public CartService(
            ICompanyRepository companyRepository,
            ICartRepository cartRepository,
            ICheckoutService checkoutService)
        {
            _companyRepository = companyRepository;
            _cartRepository = cartRepository;
            _checkoutService = checkoutService;
        }

        public async Task<CartDto> AddInvoiceToCartAsync(Guid companyId, Guid invoiceId)
        {
            await ValidateCompanyExists(companyId);
            var invoice = await GetInvoiceById(invoiceId);
            
            ValidateInvoiceBelongsToCompany(invoice, companyId);
            ValidateInvoiceNotAlreadyInCart(invoice);

            var company = await _companyRepository.GetByIdAsync(companyId);
            var totalInCart = await _cartRepository.GetCartTotalAsync(companyId);
            var newTotal = totalInCart + invoice.Amount;

            if (newTotal > company!.CreditLimit)
            {
                throw new InvalidOperationException(
                    $"Adicionar esta nota fiscal excederia o limite de crédito. " +
                    $"Valor atual no carrinho: R$ {totalInCart:N2}, " +
                    $"Valor da nota: R$ {invoice.Amount:N2}, " +
                    $"Total: R$ {newTotal:N2}, " +
                    $"Limite: R$ {company.CreditLimit:N2}");
            }

            await _cartRepository.AddInvoiceToCartAsync(invoiceId);
            await _cartRepository.SaveChangesAsync();

            return await GetCartAsync(companyId);
        }

        public async Task<CartDto> RemoveInvoiceFromCartAsync(Guid companyId, Guid invoiceId)
        {
            await ValidateCompanyExists(companyId);
            var invoice = await GetInvoiceById(invoiceId);
            
            ValidateInvoiceBelongsToCompany(invoice, companyId);
            ValidateInvoiceIsInCart(invoice);

            await _cartRepository.RemoveInvoiceFromCartAsync(invoiceId);
            await _cartRepository.SaveChangesAsync();

            return await GetCartAsync(companyId);
        }

        public async Task<CartDto> GetCartAsync(Guid companyId)
        {
            var company = await ValidateCompanyExists(companyId);
            var invoicesInCart = await _cartRepository.GetCartItemsByCompanyAsync(companyId);
            var invoicesList = invoicesInCart.ToList();

            return new CartDto
            {
                CompanyId = companyId,
                CompanyName = company.Name,
                Invoices = invoicesList.Select(MapToCartInvoiceDto).ToList(),
                TotalItems = invoicesList.Count,
                TotalGross = invoicesList.Sum(i => i.Amount)
            };
        }

        public async Task<CheckoutDto> GetCartCheckoutAsync(Guid companyId)
        {
            await ValidateCompanyExists(companyId);
            return await _checkoutService.CalculateCheckoutAsync(companyId);
        }

        private async Task<receivables.Infrastructure.Entities.Company> ValidateCompanyExists(Guid companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            
            if (company == null)
            {
                throw new InvalidOperationException($"Empresa com ID {companyId} não foi encontrada");
            }

            return company;
        }

        private async Task<receivables.Infrastructure.Entities.Invoice> GetInvoiceById(Guid invoiceId)
        {
            var invoice = await _cartRepository.GetInvoiceByIdAsync(invoiceId);
            
            if (invoice == null)
            {
                throw new InvalidOperationException($"Nota fiscal com ID {invoiceId} não foi encontrada");
            }

            return invoice;
        }

        private static void ValidateInvoiceBelongsToCompany(receivables.Infrastructure.Entities.Invoice invoice, Guid companyId)
        {
            if (invoice.CompanyId != companyId)
            {
                throw new InvalidOperationException("A nota fiscal não pertence a esta empresa");
            }
        }

        private static void ValidateInvoiceNotAlreadyInCart(receivables.Infrastructure.Entities.Invoice invoice)
        {
            if (invoice.InCart)
            {
                throw new InvalidOperationException("A nota fiscal já está no carrinho");
            }
        }

        private static void ValidateInvoiceIsInCart(receivables.Infrastructure.Entities.Invoice invoice)
        {
            if (!invoice.InCart)
            {
                throw new InvalidOperationException("A nota fiscal não está no carrinho");
            }
        }

        private static CartInvoiceDto MapToCartInvoiceDto(receivables.Infrastructure.Entities.Invoice invoice)
        {
            return new CartInvoiceDto
            {
                Id = invoice.Id,
                Number = invoice.Number,
                Amount = invoice.Amount,
                DueDate = invoice.DueDate
            };
        }
    }
}
