namespace PAS.Domain.Funds;

// public readonly record struct ?
public record FundId(Guid Value) {
    public static FundId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(FundId id) => id.Value;

    public static explicit operator FundId(Guid value) => new(value);
}