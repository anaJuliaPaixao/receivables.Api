using Receivables.Domain.Enums;
using Receivables.Domain.Services;

namespace receivables.Api.Tests;

public class CreditLimitCalculatorTests
{
    private readonly CreditLimitCalculator _calculator;

    public CreditLimitCalculatorTests()
    {
        _calculator = new CreditLimitCalculator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5000)]
    [InlineData(9999.99)]
    public void Calculate_WhenRevenueIsBelowMinimum_ShouldReturnZero(decimal revenue)
    {
        // Arrange & Act
        var resultServices = _calculator.Calculate(revenue, CompanySegment.Services);
        var resultProducts = _calculator.Calculate(revenue, CompanySegment.Products);

        // Assert
        Assert.Equal(0m, resultServices);
        Assert.Equal(0m, resultProducts);
    }

    [Theory]
    [InlineData(10000, CompanySegment.Services, 5000)]
    [InlineData(10000, CompanySegment.Products, 5000)]
    [InlineData(30000, CompanySegment.Services, 15000)]
    [InlineData(30000, CompanySegment.Products, 15000)]
    [InlineData(50000, CompanySegment.Services, 25000)]
    [InlineData(50000, CompanySegment.Products, 25000)]
    public void Calculate_WhenRevenueInBracket1_ShouldReturn50Percent(decimal revenue, CompanySegment segment, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(revenue, segment);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(50001, CompanySegment.Services, 27500.55)]
    [InlineData(75000, CompanySegment.Services, 41250)]
    [InlineData(100000, CompanySegment.Services, 55000)]
    public void Calculate_WhenRevenueInBracket2Services_ShouldReturn55Percent(decimal revenue, CompanySegment segment, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(revenue, segment);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(50001, CompanySegment.Products, 30000.60)]
    [InlineData(75000, CompanySegment.Products, 45000)]
    [InlineData(100000, CompanySegment.Products, 60000)]
    public void Calculate_WhenRevenueInBracket2Products_ShouldReturn60Percent(decimal revenue, CompanySegment segment, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(revenue, segment);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(100001, CompanySegment.Services, 60000.60)]
    [InlineData(150000, CompanySegment.Services, 90000)]
    [InlineData(200000, CompanySegment.Services, 120000)]
    public void Calculate_WhenRevenueInBracket3Services_ShouldReturn60Percent(decimal revenue, CompanySegment segment, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(revenue, segment);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(100001, CompanySegment.Products, 65000.65)]
    [InlineData(150000, CompanySegment.Products, 97500)]
    [InlineData(200000, CompanySegment.Products, 130000)]
    public void Calculate_WhenRevenueInBracket3Products_ShouldReturn65Percent(decimal revenue, CompanySegment segment, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(revenue, segment);

        // Assert
        Assert.Equal(expected, result);
    }
}
