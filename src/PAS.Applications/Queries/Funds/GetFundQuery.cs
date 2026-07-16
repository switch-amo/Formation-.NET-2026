using MediatR;

namespace PAS.Application.Queries.Funds;

// Returns null when the fund does not exist; the endpoint maps that to 404.
public sealed record GetFundQuery(Guid Id) : IRequest<FundDto?>;

// Detailed read model, including the full NAV history.
public sealed record FundDto(Guid Id, string Name, string Isin, string Currency, string Status, IReadOnlyList<NavDto> Navs);

public sealed record NavDto(DateOnly Date, decimal Value);