using PAS.Domain.Abstractions;
using PAS.Domain.Funds.Enums;
using PAS.Domain.Funds.Events;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.Domain.Funds;

public sealed class Fund : AggregateRoot<FundId> {
    private readonly List<Nav> _navs = new();

    public string Name { get; private set; } = null!;
    public Isin Isin { get; private set; } = null!;
    public Currency Currency { get; private set; } = null!;
    public FundStatus Status { get; private set; }
    public IReadOnlyCollection<Nav> Navs => _navs.AsReadOnly();

    private Fund() { } // required by EF Core

    public static Fund Create(string name, Isin isin, Currency currency) {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Fund name is required");

        return new Fund {
            Id = FundId.New(),
            Name = name.Trim(),
            Isin = isin,
            Currency = currency,
            Status = FundStatus.Active
        };
    }

    public void AddNav(DateOnly date, decimal value, DateOnly today) {
        if (Status == FundStatus.Closed) throw new DomainException("Cannot register a NAV on a closed fund");

        var nav = Nav.Create(date, value, today);

        _navs.RemoveAll(n => n.Date == date);
        _navs.Add(nav);

        Raise(new FundNavUpdatedDomainEvent(Id, Isin.Value, date, value));
    }

    public void Suspend() => TransitionTo(FundStatus.Suspended);
    public void Reactivate() => TransitionTo(FundStatus.Active);
    public void Close() => TransitionTo(FundStatus.Closed);

    private void TransitionTo(FundStatus target) {
        if (Status == target) return; // already in the target state — nothing to do

        if (Status == FundStatus.Closed)
            throw new DomainException($"A closed fund is terminal and cannot become {target}");

        var previous = Status;
        Status = target;

        Raise(new FundStatusChangedDomainEvent(Id, previous, target));
    }
}