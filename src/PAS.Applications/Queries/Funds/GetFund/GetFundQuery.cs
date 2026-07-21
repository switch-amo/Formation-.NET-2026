using MediatR;

namespace PAS.Application.Queries.Funds.GetFund;

public sealed record GetFundQuery(Guid Id) : IRequest<FundDto?>;
public sealed record FundDto(Guid Id, string Name, string Isin, string Currency, string Status, IReadOnlyList<NavDto> Navs);
public sealed record NavDto(DateOnly Date, decimal Value);