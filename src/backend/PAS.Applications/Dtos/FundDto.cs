namespace PAS.Application.Dtos;

public sealed record FundDto(Guid Id, string Name, string Isin, string Currency, string Status, IReadOnlyList<NavDto> Navs);