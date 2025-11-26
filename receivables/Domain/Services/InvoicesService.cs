using receivables.Domain.DTOs.Invoice;
using receivables.Domain.Interfaces;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;

namespace receivables.Domain.Services
{
    public class InvoicesService : IInvoicesService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICompanyRepository _companyRepository;

        public InvoicesService(
            IInvoiceRepository invoiceRepository,
            ICompanyRepository companyRepository)
        {
            _invoiceRepository = invoiceRepository;
            _companyRepository = companyRepository;
        }

        public async Task<InvoiceDto> CreateAsync(InvoiceRequest invoiceRequest)
        {
            await ValidateCompanyExists(invoiceRequest.CompanyId);
            
            await ValidateInvoiceDoesNotExist(invoiceRequest.Number, invoiceRequest.CompanyId);

            var invoice = CreateInvoiceEntity(invoiceRequest);

            await PersistInvoice(invoice);

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> UpdateAsync(Guid id, UpdateInvoiceRequest updateRequest)
        {
            var invoice = await GetInvoiceById(id);
            
            await ValidateInvoiceNumberIsNotDuplicate(updateRequest.Number, invoice);
            
            ValidateInvoiceIsNotInCart(invoice);

            UpdateInvoiceData(invoice, updateRequest);

            await PersistUpdate(invoice);

            return MapToDto(invoice);
        }

        private async Task<Invoice> GetInvoiceById(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            
            if (invoice == null)
            {
                throw new InvalidOperationException($"Nota fiscal com ID {id} não foi encontrada");
            }

            return invoice;
        }

        private async Task ValidateCompanyExists(Guid companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            
            if (company == null)
            {
                throw new InvalidOperationException($"Empresa com ID {companyId} não foi encontrada");
            }
        }

        private async Task ValidateInvoiceDoesNotExist(string number, Guid companyId)
        {
            var existingInvoice = await _invoiceRepository.GetByNumberAndCompanyAsync(number, companyId);
            
            if (existingInvoice != null)
            {
                throw new InvalidOperationException($"Já existe uma nota fiscal com o número {number} para esta empresa");
            }
        }

        private async Task ValidateInvoiceNumberIsNotDuplicate(string number, Invoice currentInvoice)
        {
            if (currentInvoice.Number == number)
                return;

            var existingInvoice = await _invoiceRepository.GetByNumberAndCompanyAsync(number, currentInvoice.CompanyId);
            
            if (existingInvoice != null && existingInvoice.Id != currentInvoice.Id)
            {
                throw new InvalidOperationException($"Já existe outra nota fiscal com o número {number} para esta empresa");
            }
        }

        private static void ValidateInvoiceIsNotInCart(Invoice invoice)
        {
            if (invoice.InCart)
            {
                throw new InvalidOperationException("Não é possível editar uma nota fiscal que está no carrinho");
            }
        }

        private static Invoice CreateInvoiceEntity(InvoiceRequest request)
        {
            return new Invoice(
                companyId: request.CompanyId,
                number: request.Number,
                amount: request.Amount,
                dueDate: request.DueDate
            );
        }

        private static void UpdateInvoiceData(Invoice invoice, UpdateInvoiceRequest request)
        {
            invoice.Update(
                number: request.Number,
                amount: request.Amount,
                dueDate: request.DueDate
            );
        }

        private async Task PersistInvoice(Invoice invoice)
        {
            await _invoiceRepository.AddAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();
        }

        private async Task PersistUpdate(Invoice invoice)
        {
            _invoiceRepository.Update(invoice);
            await _invoiceRepository.SaveChangesAsync();
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                CompanyId = invoice.CompanyId,
                Number = invoice.Number,
                Amount = invoice.Amount,
                DueDate = invoice.DueDate,
                InCart = invoice.InCart
            };
        }
    }
}
