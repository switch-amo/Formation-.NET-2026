using MediatR;

namespace PAS.Application.Commands.Funds.CreateFund;

public sealed record CreateFundCommand(string Name, string Isin, string Currency) : IRequest<Guid>;