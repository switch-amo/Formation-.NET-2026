using MediatR;
using PAS.Application.Dtos;

namespace PAS.Application.Queries.GetFundList;

public sealed record GetFundListQuery() : IRequest<IReadOnlyCollection<FundDto>>;