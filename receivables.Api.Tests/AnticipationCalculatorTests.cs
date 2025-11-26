using Receivables.Domain.Services;

namespace receivables.Api.Tests;

public class AnticipationCalculatorTests
{
    private readonly AnticipationCalculator _calculator;

    public AnticipationCalculatorTests()
    {
        _calculator = new AnticipationCalculator();
    }

    [Fact]
    public void CalculateNetValue_WhenDueDateIsToday_ShouldReturnGrossValue()
    {
        // Arrange
        var grossValue = 1000m;
        var dueDate = new DateTime(2024, 1, 15);
        var now = new DateTime(2024, 1, 15);

        // Act
        var result = _calculator.CalculateNetValue(grossValue, dueDate, now);

        // Assert
        Assert.Equal(grossValue, result);
    }

    [Fact]
    public void CalculateNetValue_WhenDueDateIsInPast_ShouldReturnGrossValue()
    {
        // Arrange
        var grossValue = 1000m;
        var dueDate = new DateTime(2024, 1, 10);
        var now = new DateTime(2024, 1, 15);

        // Act
        var result = _calculator.CalculateNetValue(grossValue, dueDate, now);

        // Assert
        Assert.Equal(grossValue, result);
    }

    [Fact]
    public void CalculateNetValue_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange
        var grossValue = 1234.567m;
        var dueDate = new DateTime(2024, 2, 15);
        var now = new DateTime(2024, 1, 15);

        // Act
        var result = _calculator.CalculateNetValue(grossValue, dueDate, now);

        // Assert
        Assert.Equal(2, BitConverter.GetBytes(decimal.GetBits(result)[3])[2]);
    }
}
