using Moq;
using receivables.Domain.Services;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;
using Receivables.Domain.Enums;
using Receivables.Domain.Services;
using receivables.Domain.Interfaces;

namespace receivables.Api.Tests;

public class CartServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<ICheckoutService> _checkoutServiceMock;
    private readonly CartService _service;

    public CartServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _cartRepositoryMock = new Mock<ICartRepository>();
        _checkoutServiceMock = new Mock<ICheckoutService>();
        _service = new CartService(
            _companyRepositoryMock.Object,
            _cartRepositoryMock.Object,
            _checkoutServiceMock.Object);
    }

    [Fact]
    public async Task AddInvoiceToCartAsync_WhenValidRequest_ShouldAddInvoice()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        _cartRepositoryMock.Setup(x => x.GetCartTotalAsync(companyId))
            .ReturnsAsync(5000m);

        _cartRepositoryMock.Setup(x => x.AddInvoiceToCartAsync(invoiceId))
            .Returns(Task.CompletedTask);

        _cartRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        _cartRepositoryMock.Setup(x => x.GetCartItemsByCompanyAsync(companyId))
            .ReturnsAsync(new List<Invoice> { invoice });

        // Act
        var result = await _service.AddInvoiceToCartAsync(companyId, invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(companyId, result.CompanyId);
        _cartRepositoryMock.Verify(x => x.AddInvoiceToCartAsync(invoiceId), Times.Once);
        _cartRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddInvoiceToCartAsync_WhenCompanyNotFound_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync((Company?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddInvoiceToCartAsync(companyId, invoiceId));
        Assert.Contains("não foi encontrada", exception.Message);

        _cartRepositoryMock.Verify(x => x.AddInvoiceToCartAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AddInvoiceToCartAsync_WhenInvoiceNotFound_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
            .ReturnsAsync((Invoice?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddInvoiceToCartAsync(companyId, invoiceId));
        Assert.Contains("Nota fiscal com ID", exception.Message);

        _cartRepositoryMock.Verify(x => x.AddInvoiceToCartAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AddInvoiceToCartAsync_WhenInvoiceDoesNotBelongToCompany_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var otherCompanyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        var invoice = new Invoice(otherCompanyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddInvoiceToCartAsync(companyId, invoiceId));
        Assert.Contains("não pertence a esta empresa", exception.Message);

        _cartRepositoryMock.Verify(x => x.AddInvoiceToCartAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AddInvoiceToCartAsync_WhenInvoiceAlreadyInCart_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));
        invoice.AddToCart();

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddInvoiceToCartAsync(companyId, invoiceId));
        Assert.Contains("já está no carrinho", exception.Message);

        _cartRepositoryMock.Verify(x => x.AddInvoiceToCartAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AddInvoiceToCartAsync_WhenExceedsCreditLimit_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        var invoice = new Invoice(companyId, "INV001", 20000m, DateTime.UtcNow.AddDays(30));

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        _cartRepositoryMock.Setup(x => x.GetCartTotalAsync(companyId))
            .ReturnsAsync(10000m);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddInvoiceToCartAsync(companyId, invoiceId));
        Assert.Contains("excederia o limite de crédito", exception.Message);

        _cartRepositoryMock.Verify(x => x.AddInvoiceToCartAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RemoveInvoiceFromCartAsync_WhenValidRequest_ShouldRemoveInvoice()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));
        invoice.AddToCart();

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        _cartRepositoryMock.Setup(x => x.RemoveInvoiceFromCartAsync(invoiceId))
            .Returns(Task.CompletedTask);

        _cartRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        _cartRepositoryMock.Setup(x => x.GetCartItemsByCompanyAsync(companyId))
            .ReturnsAsync(new List<Invoice>());

        // Act
        var result = await _service.RemoveInvoiceFromCartAsync(companyId, invoiceId);

        // Assert
        Assert.NotNull(result);
        _cartRepositoryMock.Verify(x => x.RemoveInvoiceFromCartAsync(invoiceId), Times.Once);
        _cartRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveInvoiceFromCartAsync_WhenInvoiceNotInCart_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RemoveInvoiceFromCartAsync(companyId, invoiceId));
        Assert.Contains("não está no carrinho", exception.Message);

        _cartRepositoryMock.Verify(x => x.RemoveInvoiceFromCartAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetCartAsync_WhenValidRequest_ShouldReturnCartDto()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        
        var invoices = new List<Invoice>
        {
            new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30)),
            new Invoice(companyId, "INV002", 5000m, DateTime.UtcNow.AddDays(60))
        };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetCartItemsByCompanyAsync(companyId))
            .ReturnsAsync(invoices);

        // Act
        var result = await _service.GetCartAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(companyId, result.CompanyId);
        Assert.Equal("Test Company", result.CompanyName);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(15000m, result.TotalGross);
        Assert.Equal(2, result.Invoices.Count);
    }

    [Fact]
    public async Task GetCartAsync_WhenCartIsEmpty_ShouldReturnEmptyCart()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _cartRepositoryMock.Setup(x => x.GetCartItemsByCompanyAsync(companyId))
            .ReturnsAsync(new List<Invoice>());

        // Act
        var result = await _service.GetCartAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0m, result.TotalGross);
    }
}
