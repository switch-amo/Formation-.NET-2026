namespace PAS.Domain.Abstractions;

// Base for entities: identity equality.
// Two entities are equal if and only if they share the same Id,
// regardless of their other attributes.
public abstract class Entity {
    public Guid Id { get; protected init; }
}