using FluentAssertions;
using PAS.Domain.Abstractions;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.UnitTests.Domain.Funds.ValueObjects;

public class NavTests {
    private static readonly DateOnly Today = new(2025, 6, 15);

    [Fact]
    public void Create_WithValidData_ShouldReturnNav() {
        // Arrange
        var date = new DateOnly(2025, 1, 1);
        var value = 125.50m;

        // Act
        var nav = Nav.Create(date, value, Today);

        // Assert
        nav.Date.Should().Be(date);
        nav.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithNonPositiveValue_ShouldThrowDomainException(decimal value) {
        // Arrange
        var date = new DateOnly(2025, 1, 1);

        // Act
        var action = () => Nav.Create(date, value, Today);

        // Assert
        action.Should().Throw<DomainException>().WithMessage("NAV value must be strictly positive");
    }

    [Fact]
    public void Create_WithFutureDate_ShouldThrowDomainException() {
        // Arrange — one day after the fixed clock.
        var futureDate = new DateOnly(2025, 6, 16);

        // Act
        var action = () => Nav.Create(futureDate, 100m, Today);

        // Assert
        action.Should().Throw<DomainException>().WithMessage("NAV date cannot be in the future");
    }

    [Fact]
    public void Create_WithTodayDate_ShouldReturnNav() {
        // Act — a valuation date equal to "today" is allowed.
        var nav = Nav.Create(Today, 100m, Today);

        // Assert
        nav.Date.Should().Be(Today);
        nav.Value.Should().Be(100m);
    }
}