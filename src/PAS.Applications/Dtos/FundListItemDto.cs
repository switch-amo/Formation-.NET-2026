namespace PAS.Application.Dtos;

// Read model for the list endpoint. Flat, display-oriented.
// Primitive types only — no domain Value Object leaks across the boundary.
public sealed record FundListItemDto(
    Guid Id,
    string Name,
    string Isin,
    string Currency,
    string Status,
    decimal? LatestNav);