using Moq;
using receivables.Domain.Services;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;
using Receivables.Domain.Enums;
using Receivables.Domain.Services;

namespace receivables.Api.Tests;

public class CheckoutServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<AnticipationCalculator> _anticipationCalculatorMock;
    private readonly CheckoutService _service;

    public CheckoutServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _anticipationCalculatorMock = new Mock<AnticipationCalculator>();
        _service = new CheckoutService(
            _companyRepositoryMock.Object,
            _invoiceRepositoryMock.Object,
            _anticipationCalculatorMock.Object);
    }

    [Fact]
    public async Task CalculateCheckoutAsync_WhenCompanyNotFound_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync((Company?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.CalculateCheckoutAsync(companyId));
        Assert.Contains("não foi encontrada", exception.Message);
    }

    [Fact]
    public async Task CalculateCheckoutAsync_WhenCartIsEmpty_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _invoiceRepositoryMock.Setup(x => x.GetInCartByCompanyAsync(companyId))
            .ReturnsAsync(new List<Invoice>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.CalculateCheckoutAsync(companyId));
        Assert.Contains("Não há notas fiscais no carrinho", exception.Message);
    }

    [Fact]
    public async Task CalculateCheckoutByInvoiceIdAsync_WhenInvoiceNotFound_ShouldThrowException()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync((Invoice?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.CalculateCheckoutByInvoiceIdAsync(invoiceId));
        Assert.Contains("não foi encontrada", exception.Message);
    }
}
