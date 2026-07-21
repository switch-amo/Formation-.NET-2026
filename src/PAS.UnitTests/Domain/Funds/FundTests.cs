using FluentAssertions;
using PAS.Domain.Abstractions;
using PAS.Domain.Funds;
using PAS.Domain.Funds.Enums;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.UnitTests.Domain.Funds;

public class FundTests {
    [Fact]
    public void Create_ShouldCreateActiveFund() {
        // Arrange
        var name = "Global Equity";
        var isin = Isin.Create("FR1234567890");
        var currency = Currency.Create("EUR");

        // Act
        var fund = Fund.Create(name, isin, currency);

        // Assert
        fund.Name.Should().Be(name);
        fund.Isin.Should().Be(isin);
        fund.Currency.Should().Be(currency);
        fund.Status.Should().Be(FundStatus.Active);
        fund.Navs.Should().BeEmpty();
        fund.LatestNav.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException() {
        // Arrange
        var isin = Isin.Create("FR1234567890");
        var currency = Currency.Create("EUR");

        // Act
        var action = () => Fund.Create("", isin, currency);

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddNav_ShouldAddNav() {
        // Arrange
        var fund = Fund.Create(
            "Global Equity",
            Isin.Create("FR1234567890"),
            Currency.Create("EUR"));

        // Act
        fund.AddNav(new DateOnly(2025, 1, 1), 125.50m);

        // Assert
        fund.Navs.Should().HaveCount(1);
        fund.LatestNav.Should().NotBeNull();
        fund.LatestNav!.Value.Should().Be(125.50m);
    }
}