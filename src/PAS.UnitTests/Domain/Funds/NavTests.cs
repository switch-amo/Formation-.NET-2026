using FluentAssertions;
using PAS.Domain.Abstractions;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.UnitTests.Domain.Funds.ValueObjects;

public class NavTests {
    [Fact]
    public void Create_WithValidData_ShouldReturnNav() {
        // Arrange
        var date = new DateOnly(2025, 1, 1);
        var value = 125.50m;

        // Act
        var nav = Nav.Create(date, value);

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
        var action = () => Nav.Create(date, value);

        // Assert
        action.Should().Throw<DomainException>().WithMessage("NAV value must be strictly positive");
    }

    [Fact]
    public void Create_WithFutureDate_ShouldThrowDomainException() {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act
        var action = () => Nav.Create(futureDate, 100m);

        // Assert
        action.Should().Throw<DomainException>().WithMessage("NAV date cannot be in the future");
    }

    [Fact]
    public void Create_WithTodayDate_ShouldReturnNav() {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var nav = Nav.Create(today, 100m);

        // Assert
        nav.Date.Should().Be(today);
        nav.Value.Should().Be(100m);
    }
}