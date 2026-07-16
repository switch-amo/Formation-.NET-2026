using MediatR;
using PAS.Application.Dtos;   

namespace PAS.Application.Queries.Funds;

public sealed record GetFundListQuery : IRequest<IReadOnlyList<FundListItemDto>>;