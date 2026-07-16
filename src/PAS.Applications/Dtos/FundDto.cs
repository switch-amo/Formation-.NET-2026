namespace PAS.Application.Dtos;

// Detailed read model for the single-fund endpoint, with full NAV history.
public sealed record FundDto(
    Guid Id,
    string Name,
    string Isin,
    string Currency,
    string Status,
    IReadOnlyList<NavDto> Navs);