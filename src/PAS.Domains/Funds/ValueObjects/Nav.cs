using PAS.Domain.Abstractions;

namespace PAS.Domain.Funds.ValueObjects;

public sealed record Nav {
    public DateOnly Date { get; }
    public decimal Value { get; }

    private Nav(DateOnly date, decimal value) {
        Date = date;
        Value = value;
    }

    public static Nav Create(DateOnly date, decimal value, DateOnly today) {
        if (value <= 0) throw new DomainException("NAV value must be strictly positive");

        if (date > today) throw new DomainException("NAV date cannot be in the future");

        return new Nav(date, value);
    }
}