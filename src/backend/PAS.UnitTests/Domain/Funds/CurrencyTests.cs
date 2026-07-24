using FluentAssertions;
using PAS.Domain.Abstractions;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.UnitTests.Domain.Funds.ValueObjects;

public class CurrencyTests {
    [Fact]
    public void Create_WithValidCurrency_ShouldReturnCurrency() {
        // Arrange
        var code = "EUR";

        // Act
        var currency = Currency.Create(code);

        // Assert
        currency.Code.Should().Be("EUR");
    }

    [Fact]
    public void Create_ShouldConvertCodeToUpperCase() {
        // Arrange
        var code = "eur";

        // Act
        var currency = Currency.Create(code);

        // Assert
        currency.Code.Should().Be("EUR");
    }

    [Fact]
    public void Create_ShouldTrimWhitespace() {
        // Arrange
        var code = "  EUR  ";

        // Act
        var currency = Currency.Create(code);

        // Assert
        currency.Code.Should().Be("EUR");
    }

    [Fact]
    public void Create_WithEmptyCode_ShouldThrowDomainException() {
        // Arrange
        var code = string.Empty;

        // Act
        var action = () => Currency.Create(code);

        // Assert
        action.Should()
            .Throw<DomainException>()
            .WithMessage("Currency is required");
    }

    [Fact]
    public void Create_WithNullCode_ShouldThrowDomainException() {
        // Arrange
        string? code = null;

        // Act
        var action = () => Currency.Create(code!);

        // Assert
        action.Should()
            .Throw<DomainException>()
            .WithMessage("Currency is required");
    }

    [Theory]
    [InlineData("EU")]
    [InlineData("EURO")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("E2R")]
    [InlineData("EU1")]
    [InlineData("€UR")]
    public void Create_WithInvalidCode_ShouldThrowDomainException(string code) {
        // Act
        var action = () => Currency.Create(code);

        // Assert
        action.Should()
            .Throw<DomainException>()
            .WithMessage("Currency must be a 3-letter ISO 4217 code");
    }
}