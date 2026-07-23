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

        if (!HasValidChecksum(value))
            throw new DomainException("ISIN has an invalid check digit");

        return new Isin(value);
    }

    // ISO 6166 check digit: expand letters (A=10 … Z=35) then apply the Luhn algorithm.
    private static bool HasValidChecksum(string isin) {
        var digits = new List<int>(24);
        foreach (var c in isin) {
            var v = char.IsDigit(c) ? c - '0' : c - 'A' + 10;
            if (v >= 10) {
                digits.Add(v / 10);
                digits.Add(v % 10);
            } else {
                digits.Add(v);
            }
        }

        var sum = 0;
        var doubleDigit = false;
        for (var i = digits.Count - 1; i >= 0; i--) {
            var d = digits[i];
            if (doubleDigit) {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleDigit = !doubleDigit;
        }

        return sum % 10 == 0;
    }

    public override string ToString() => Value;
}