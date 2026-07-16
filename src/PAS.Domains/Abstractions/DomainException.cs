namespace PAS.Domain.Abstractions;

// Thrown when a business rule / invariant is violated.
public sealed class DomainException : Exception {
    public DomainException(string message) : base(message) { }
}