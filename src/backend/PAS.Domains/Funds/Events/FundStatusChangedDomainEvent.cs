using PAS.Domain.Abstractions;
using PAS.Domain.Funds.Enums;

namespace PAS.Domain.Funds.Events;

public sealed record FundStatusChangedDomainEvent(
    Guid FundId,
    FundStatus PreviousStatus,
    FundStatus NewStatus) : IDomainEvent {
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
