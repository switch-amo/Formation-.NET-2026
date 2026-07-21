using PAS.Domain.Abstractions;
using PAS.Domain.Funds.Enums;
using PAS.Domain.Funds.ValueObjects;

namespace PAS.Domain.Funds;

public sealed class Fund : AggregateRoot<FundId> {
    private readonly List<Nav> _navs = new();

    public string Name { get; private set; } = null!;
    public Isin Isin { get; private set; } = null!;
    public Currency Currency { get; private set; } = null!;
    public FundStatus Status { get; private set; }
    public IReadOnlyCollection<Nav> Navs => _navs.AsReadOnly();

    public Nav? LatestNav => _navs.MaxBy(n => n.Date);

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

    public void AddNav(DateOnly date, decimal value) {
        if (Status == FundStatus.Closed) throw new DomainException("Cannot register a NAV on a closed fund");

        var nav = Nav.Create(date, value);

        _navs.RemoveAll(n => n.Date == date);
        _navs.Add(nav);
    }

    public void Close() => Status = FundStatus.Closed;
    public void Suspend() => Status = FundStatus.Suspended;
    public void Reactivate() => Status = FundStatus.Active;
}