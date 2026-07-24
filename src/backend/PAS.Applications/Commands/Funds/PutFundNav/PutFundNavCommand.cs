using MediatR;

namespace PAS.Application.Commands.Funds.PutFundNav;

public sealed record PutFundNavCommand(Guid FundId, DateOnly Date, decimal Value) : IRequest;