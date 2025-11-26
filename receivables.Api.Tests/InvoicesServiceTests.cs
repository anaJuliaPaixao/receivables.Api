using Moq;
using receivables.Domain.DTOs.Invoice;
using receivables.Domain.Services;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;
using Receivables.Domain.Enums;
using Receivables.Domain.Services;

namespace receivables.Api.Tests;

public class InvoicesServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly InvoicesService _service;

    public InvoicesServiceTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _service = new InvoicesService(_invoiceRepositoryMock.Object, _companyRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldCreateInvoice()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);

        var request = new InvoiceRequest
        {
            CompanyId = companyId,
            Number = "INV001",
            Amount = 10000m,
            DueDate = DateTime.UtcNow.AddDays(30)
        };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _invoiceRepositoryMock.Setup(x => x.GetByNumberAndCompanyAsync(request.Number, companyId))
            .ReturnsAsync((Invoice?)null);

        _invoiceRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Invoice>()))
            .Returns(Task.CompletedTask);

        _invoiceRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.CompanyId, result.CompanyId);
        Assert.Equal(request.Number, result.Number);
        Assert.Equal(request.Amount, result.Amount);
        Assert.Equal(request.DueDate, result.DueDate);
        Assert.False(result.InCart);

        _invoiceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Invoice>()), Times.Once);
        _invoiceRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyNotFound_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var request = new InvoiceRequest
        {
            CompanyId = companyId,
            Number = "INV001",
            Amount = 10000m,
            DueDate = DateTime.UtcNow.AddDays(30)
        };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync((Company?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        Assert.Contains("não foi encontrada", exception.Message);

        _invoiceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenInvoiceNumberAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var creditLimitCalculator = new CreditLimitCalculator();
        var company = new Company("12345678901234", "Test Company", 50000m, CompanySegment.Services, creditLimitCalculator);
        var existingInvoice = new Invoice(companyId, "INV001", 5000m, DateTime.UtcNow.AddDays(30));

        var request = new InvoiceRequest
        {
            CompanyId = companyId,
            Number = "INV001",
            Amount = 10000m,
            DueDate = DateTime.UtcNow.AddDays(30)
        };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId))
            .ReturnsAsync(company);

        _invoiceRepositoryMock.Setup(x => x.GetByNumberAndCompanyAsync(request.Number, companyId))
            .ReturnsAsync(existingInvoice);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        Assert.Contains("Já existe uma nota fiscal com o número", exception.Message);

        _invoiceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenValidRequest_ShouldUpdateInvoice()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));

        var updateRequest = new UpdateInvoiceRequest
        {
            Number = "INV001-UPDATED",
            Amount = 15000m,
            DueDate = DateTime.UtcNow.AddDays(60)
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        _invoiceRepositoryMock.Setup(x => x.GetByNumberAndCompanyAsync(updateRequest.Number, companyId))
            .ReturnsAsync((Invoice?)null);

        _invoiceRepositoryMock.Setup(x => x.Update(It.IsAny<Invoice>()));

        _invoiceRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(invoiceId, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateRequest.Number, result.Number);
        Assert.Equal(updateRequest.Amount, result.Amount);
        Assert.Equal(updateRequest.DueDate, result.DueDate);

        _invoiceRepositoryMock.Verify(x => x.Update(It.IsAny<Invoice>()), Times.Once);
        _invoiceRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenInvoiceNotFound_ShouldThrowException()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var updateRequest = new UpdateInvoiceRequest
        {
            Number = "INV001",
            Amount = 15000m,
            DueDate = DateTime.UtcNow.AddDays(60)
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync((Invoice?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdateAsync(invoiceId, updateRequest));
        Assert.Contains("não foi encontrada", exception.Message);

        _invoiceRepositoryMock.Verify(x => x.Update(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenInvoiceIsInCart_ShouldThrowException()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));
        invoice.AddToCart();

        var updateRequest = new UpdateInvoiceRequest
        {
            Number = "INV001-UPDATED",
            Amount = 15000m,
            DueDate = DateTime.UtcNow.AddDays(60)
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdateAsync(invoiceId, updateRequest));
        Assert.Contains("Não é possível editar uma nota fiscal que está no carrinho", exception.Message);

        _invoiceRepositoryMock.Verify(x => x.Update(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenNewNumberAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));
        var otherInvoice = new Invoice(companyId, "INV002", 5000m, DateTime.UtcNow.AddDays(30));

        var updateRequest = new UpdateInvoiceRequest
        {
            Number = "INV002",
            Amount = 15000m,
            DueDate = DateTime.UtcNow.AddDays(60)
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        _invoiceRepositoryMock.Setup(x => x.GetByNumberAndCompanyAsync(updateRequest.Number, companyId))
            .ReturnsAsync(otherInvoice);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdateAsync(invoiceId, updateRequest));
        Assert.Contains("Já existe outra nota fiscal com o número", exception.Message);

        _invoiceRepositoryMock.Verify(x => x.Update(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenKeepingSameNumber_ShouldNotCheckDuplication()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var invoice = new Invoice(companyId, "INV001", 10000m, DateTime.UtcNow.AddDays(30));

        var updateRequest = new UpdateInvoiceRequest
        {
            Number = "INV001",
            Amount = 15000m,
            DueDate = DateTime.UtcNow.AddDays(60)
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync(invoice);

        _invoiceRepositoryMock.Setup(x => x.Update(It.IsAny<Invoice>()));

        _invoiceRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(invoiceId, updateRequest);

        // Assert
        Assert.NotNull(result);
        _invoiceRepositoryMock.Verify(x => x.GetByNumberAndCompanyAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }
}
