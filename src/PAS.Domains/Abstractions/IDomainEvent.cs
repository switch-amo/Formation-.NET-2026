namespace PAS.Domain.Abstractions;

public interface IDomainEvent {
    DateTime OccurredOnUtc { get; }
}