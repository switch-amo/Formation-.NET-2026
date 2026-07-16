using PAS.Domain.Abstractions;
using PAS.Domain.Funds.Enums;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.Domain.Funds;

// Aggregate root. It is the ONLY entry point to mutate its state,
// and it protects its own invariants.
public sealed class Fund : AggregateRoot {
    private readonly List<Nav> _navs = new();

    public string Name { get; private set; } = null!;
    public Isin Isin { get; private set; } = null!;
    public Currency Currency { get; private set; } = null!;
    public FundStatus Status { get; private set; }
    public IReadOnlyCollection<Nav> Navs => _navs.AsReadOnly();

    // Convenience projection for the list/detail endpoints
    public Nav? LatestNav => _navs.MaxBy(n => n.Date);

    private Fund() { } // required by EF Core

    // Factory: a Fund is always created valid and Active.
    public static Fund Create(string name, Isin isin, Currency currency) {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Fund name is required");

        return new Fund {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Isin = isin,
            Currency = currency,
            Status = FundStatus.Active
        };
    }

    // Behaviour behind the PutFundNav endpoint.
    public void AddNav(DateOnly date, decimal value) {
        if (Status == FundStatus.Closed) throw new DomainException("Cannot register a NAV on a closed fund");

        var nav = Nav.Create(date, value);

        // Business rule: one NAV per date — the newest replaces the previous one.
        _navs.RemoveAll(n => n.Date == date);
        _navs.Add(nav);
    }

    public void Close() => Status = FundStatus.Closed;
    public void Suspend() => Status = FundStatus.Suspended;
    public void Reactivate() => Status = FundStatus.Active;
}