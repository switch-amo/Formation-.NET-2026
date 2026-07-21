using PAS.Domain.Abstractions;

namespace PAS.Domain.Funds.Events;

public sealed record FundNavUpdatedDomainEvent(Guid FundId, string Isin, DateOnly Date, decimal Value) : IDomainEvent {
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}