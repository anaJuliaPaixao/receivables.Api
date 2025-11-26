using Moq;
using receivables.Domain.DTOs.Company;
using receivables.Domain.Services;
using receivables.Infrastructure.Entities;
using receivables.Infrastructure.Repositories;
using Receivables.Domain.Enums;
using Receivables.Domain.Services;

namespace receivables.Api.Tests;

public class CompaniesServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<ICreditLimitCalculator> _creditLimitCalculatorMock;
    private readonly CompaniesService _service;

    public CompaniesServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _creditLimitCalculatorMock = new Mock<ICreditLimitCalculator>();
        _service = new CompaniesService(_companyRepositoryMock.Object, _creditLimitCalculatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldCreateCompany()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Cnpj = "12345678901234",
            Name = "Test Company",
            MonthlyRevenue = 50000m,
            Segment = CompanySegment.Services
        };

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(request.Cnpj))
            .ReturnsAsync((Company?)null);

        _creditLimitCalculatorMock.Setup(x => x.Calculate(request.MonthlyRevenue, request.Segment))
            .Returns(25000m);

        _companyRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Company>()))
            .Returns(Task.CompletedTask);

        _companyRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Cnpj, result.Cnpj);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.MonthlyRevenue, result.MonthlyRevenue);
        Assert.Equal(request.Segment, result.Segment);
        Assert.Equal(25000m, result.CreditLimit);

        _companyRepositoryMock.Verify(x => x.GetByCnpjAsync(request.Cnpj), Times.Once);
        _companyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Company>()), Times.Once);
        _companyRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenCnpjAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Cnpj = "12345678901234",
            Name = "Test Company",
            MonthlyRevenue = 50000m,
            Segment = CompanySegment.Services
        };

        var existingCompany = new Company("12345678901234", "Existing Company", 60000m, CompanySegment.Products, _creditLimitCalculatorMock.Object);

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(request.Cnpj))
            .ReturnsAsync(existingCompany);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        Assert.Contains("Já existe uma empresa cadastrada com o CNPJ", exception.Message);

        _companyRepositoryMock.Verify(x => x.GetByCnpjAsync(request.Cnpj), Times.Once);
        _companyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Company>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5000)]
    [InlineData(9999)]
    public async Task CreateAsync_WhenRevenueIsBelowMinimum_ShouldThrowException(decimal revenue)
    {
        // Arrange
        var request = new CompanyRequest
        {
            Cnpj = "12345678901234",
            Name = "Test Company",
            MonthlyRevenue = revenue,
            Segment = CompanySegment.Services
        };

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(request.Cnpj))
            .ReturnsAsync((Company?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        Assert.Contains("O faturamento mensal deve ser no mínimo R$ 10.000,00", exception.Message);

        _companyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Company>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCreditLimitCalculatorWithCorrectParameters()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Cnpj = "12345678901234",
            Name = "Test Company",
            MonthlyRevenue = 75000m,
            Segment = CompanySegment.Products
        };

        _companyRepositoryMock.Setup(x => x.GetByCnpjAsync(request.Cnpj))
            .ReturnsAsync((Company?)null);

        _creditLimitCalculatorMock.Setup(x => x.Calculate(request.MonthlyRevenue, request.Segment))
            .Returns(45000m);

        _companyRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Company>()))
            .Returns(Task.CompletedTask);

        _companyRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateAsync(request);

        // Assert
        _creditLimitCalculatorMock.Verify(x => x.Calculate(request.MonthlyRevenue, request.Segment), Times.Once);
    }
}
