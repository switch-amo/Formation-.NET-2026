using PAS.Domain.Abstractions;

namespace PAS.Domain.Funds.ValueObjects;

// Value Object: an ISIN (ISO 6166). Immutable, compared by value.
public sealed record Isin {
    public string Value { get; }

    private Isin(string value) => Value = value;

    public static Isin Create(string value) {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("ISIN is required");

        value = value.Trim().ToUpperInvariant();

        // ISO 6166 structure: 2-letter country code + 9 alphanumeric + 1 check digit
        if (value.Length != 12)
            throw new DomainException($"ISIN must be 12 characters, got {value.Length}");

        if (!char.IsLetter(value[0]) || !char.IsLetter(value[1]))
            throw new DomainException("ISIN must start with a 2-letter country code");

        if (!value.All(char.IsLetterOrDigit))
            throw new DomainException("ISIN must be alphanumeric");

        return new Isin(value);
    }

    public override string ToString() => Value;
}