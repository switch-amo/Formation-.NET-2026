using FluentAssertions;
using PAS.Domain.Abstractions;
using PAS.Domain.Funds;
using PAS.Domain.Funds.Enums;
using PAS.Domain.Funds.Events;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.UnitTests.Domain.Funds;

public class FundTests {
    private static readonly DateOnly Today = new(2025, 6, 15);

    [Fact]
    public void Create_ShouldCreateActiveFund() {
        // Arrange
        var name = "Global Equity";
        var isin = Isin.Create("FR0000120271");
        var currency = Currency.Create("EUR");

        // Act
        var fund = Fund.Create(name, isin, currency);

        // Assert
        fund.Name.Should().Be(name);
        fund.Isin.Should().Be(isin);
        fund.Currency.Should().Be(currency);
        fund.Status.Should().Be(FundStatus.Active);
        fund.Navs.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException() {
        // Arrange
        var isin = Isin.Create("FR0000120271");
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
            Isin.Create("FR0000120271"),
            Currency.Create("EUR"));

        // Act
        fund.AddNav(new DateOnly(2025, 1, 1), 125.50m, Today);

        // Assert
        fund.Navs.Should().HaveCount(1);
    }

    [Fact]
    public void AddNav_ShouldRaiseFundNavUpdatedDomainEvent() {
        // Arrange
        var fund = Fund.Create(
            "Global Equity",
            Isin.Create("FR0000120271"),
            Currency.Create("EUR"));

        // Act
        fund.AddNav(new DateOnly(2025, 1, 1), 125.50m, Today);

        // Assert
        fund.DomainEvents.Should().ContainSingle(e => e is FundNavUpdatedDomainEvent);
    }

    [Fact]
    public void Suspend_OnActiveFund_ShouldChangeStatusToSuspended() {
        // Arrange
        var fund = CreateActiveFund();

        // Act
        fund.Suspend();

        // Assert
        fund.Status.Should().Be(FundStatus.Suspended);
        fund.DomainEvents.Should().ContainSingle(e => e is FundStatusChangedDomainEvent);
    }

    [Fact]
    public void Reactivate_OnSuspendedFund_ShouldChangeStatusToActive() {
        // Arrange
        var fund = CreateActiveFund();
        fund.Suspend();

        // Act
        fund.Reactivate();

        // Assert
        fund.Status.Should().Be(FundStatus.Active);
    }

    [Fact]
    public void Suspend_OnClosedFund_ShouldThrowDomainException() {
        // Arrange
        var fund = CreateActiveFund();
        fund.Close();

        // Act
        var action = () => fund.Suspend();

        // Assert
        action.Should().Throw<DomainException>();
        fund.Status.Should().Be(FundStatus.Closed);
    }

    [Fact]
    public void AddNav_OnClosedFund_ShouldThrowDomainException() {
        // Arrange
        var fund = CreateActiveFund();
        fund.Close();

        // Act
        var action = () => fund.AddNav(new DateOnly(2025, 1, 1), 100m, Today);

        // Assert
        action.Should().Throw<DomainException>();
    }

    private static Fund CreateActiveFund() => Fund.Create(
        "Global Equity",
        Isin.Create("FR0000120271"),
        Currency.Create("EUR"));
}