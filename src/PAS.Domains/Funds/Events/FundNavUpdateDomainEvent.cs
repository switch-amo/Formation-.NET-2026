using PAS.Domain.Abstractions;

namespace PAS.Domain.Funds.Events;

// Raised by the Fund aggregate whenever a NAV is registered.
// Carries the minimum data consumers need — not the whole aggregate.
public sealed record FundNavUpdatedDomainEvent(Guid FundId, string Isin, DateOnly Date, decimal Value) : IDomainEvent {
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}