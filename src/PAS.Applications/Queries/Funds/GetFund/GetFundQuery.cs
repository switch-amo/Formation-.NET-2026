using MediatR;
using PAS.Application.Dtos;

namespace PAS.Application.Queries.Funds.GetFund;

public sealed record GetFundQuery(Guid Id) : IRequest<FundDto?>;