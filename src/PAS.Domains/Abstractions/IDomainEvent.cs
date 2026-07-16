namespace PAS.Domain.Abstractions;

// Marker interface for domain events. Kept framework-agnostic on purpose:
// the Domain must NOT depend on MediatR. Bridging to MediatR happens later,
// in Infrastructure, when we dispatch the events.
public interface IDomainEvent {
    DateTime OccurredOnUtc { get; }
}