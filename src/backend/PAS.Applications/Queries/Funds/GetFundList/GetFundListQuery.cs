using MediatR;
using PAS.Application.Dtos;

namespace PAS.Application.Queries.Funds.GetFundList;

public sealed record GetFundListQuery : IRequest<IReadOnlyList<FundListItemDto>>;