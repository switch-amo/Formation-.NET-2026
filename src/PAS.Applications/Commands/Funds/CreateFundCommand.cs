// Application/Funds/Commands/CreateFund/CreateFundCommand.cs
using MediatR;

namespace PAS.Asset.Application.Funds.Commands.CreateFund;

// Returns the Id of the created fund.
public sealed record CreateFundCommand(string Name, string Isin, string Currency) : IRequest<Guid>;