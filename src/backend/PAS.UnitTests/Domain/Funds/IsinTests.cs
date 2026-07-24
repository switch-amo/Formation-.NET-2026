using FluentAssertions;
using PAS.Domain.Abstractions;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.UnitTests.Domain.Funds.ValueObjects;

public class IsinTests {
    [Fact]
    public void Create_WithValidValue_ShouldReturnIsin() {
        // Arrange
        var value = "FR0000120271";

        // Act
        var isin = Isin.Create(value);

        // Assert
        isin.Value.Should().Be("FR0000120271");
    }

    [Fact]
    public void Create_ShouldConvertToUpperCase() {
        // Arrange
        var value = "fr0000120271";

        // Act
        var isin = Isin.Create(value);

        // Assert
        isin.Value.Should().Be("FR0000120271");
    }

    [Fact]
    public void Create_ShouldTrimValue() {
        // Arrange
        var value = "  FR0000120271  ";

        // Act
        var isin = Isin.Create(value);

        // Assert
        isin.Value.Should().Be("FR0000120271");
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowDomainException() {
        // Act
        var action = () => Isin.Create("");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("ISIN is required");
    }

    [Fact]
    public void Create_WithInvalidLength_ShouldThrowDomainException() {
        // Act
        var action = () => Isin.Create("FR123");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("ISIN must be 12 characters*");
    }

    [Fact]
    public void Create_WithInvalidCountryCode_ShouldThrowDomainException() {
        // Act
        var action = () => Isin.Create("121234567890");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("ISIN must start with a 2-letter country code");
    }

    [Fact]
    public void Create_WithSpecialCharacters_ShouldThrowDomainException() {
        // Act
        var action = () => Isin.Create("FR12345-7890");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("ISIN must be alphanumeric");
    }

    [Fact]
    public void Create_WithInvalidCheckDigit_ShouldThrowDomainException() {
        // Arrange — structurally valid but the ISO 6166 check digit is wrong.
        var value = "FR1234567890";

        // Act
        var action = () => Isin.Create(value);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("ISIN has an invalid check digit");
    }
}