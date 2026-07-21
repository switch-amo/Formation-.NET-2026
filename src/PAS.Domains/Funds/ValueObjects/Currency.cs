using PAS.Domain.Abstractions;

namespace PAS.Domain.Funds.ValueObjects;

public sealed record Currency {
    public string Code { get; }

    private Currency(string code) => Code = code;

    public static Currency Create(string code) {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Currency is required");

        code = code.Trim().ToUpperInvariant();

        if (code.Length != 3 || !code.All(char.IsLetter))
            throw new DomainException("Currency must be a 3-letter ISO 4217 code");

        return new Currency(code);
    }

    public override string ToString() => Code;
}