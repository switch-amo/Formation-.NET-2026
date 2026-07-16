// Application/Funds/Commands/PutFundNav/PutFundNavCommand.cs
using MediatR;

namespace PAS.Asset.Application.Funds.Commands.PutFundNav;

// No response body: IRequest (not IRequest<T>).
public sealed record PutFundNavCommand(Guid FundId, DateOnly Date, decimal Value) : IRequest;