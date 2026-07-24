using PAS.Domain.Abstractions;

namespace PAS.Domain.Funds.ValueObjects;

public sealed record Isin {
    public string Value { get; }

    private Isin(string value) => Value = value;

    public static Isin Create(string value) {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("ISIN is required");

        value = value.Trim().ToUpperInvariant();

        if (value.Length != 12)
            throw new DomainException($"ISIN must be 12 characters, got {value.Length}");

        if (!char.IsLetter(value[0]) || !char.IsLetter(value[1]))
            throw new DomainException("ISIN must start with a 2-letter country code");

        if (!value.All(char.IsLetterOrDigit))
            throw new DomainException("ISIN must be alphanumeric");

        return new Isin(value);
    }
}