namespace PAS.Application.Dtos;

public sealed class FundDto {
    public string Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Isin { get; init; } = string.Empty;

    public string Currency { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}