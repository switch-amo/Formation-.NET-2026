// Application/Funds/Commands/CreateFund/CreateFundHandler.cs
using MediatR;
using PAS.Domain.Funds;
using PAS.Domain.Funds.ValueObjects;
using PAS.Domain.Repositories;

namespace PAS.Asset.Application.Funds.Commands.CreateFund;

public sealed class CreateFundHandler : IRequestHandler<CreateFundCommand, Guid> {
    private readonly IFundRepository _repository;

    public CreateFundHandler(IFundRepository repository) => _repository = repository;

    public async Task<Guid> Handle(CreateFundCommand request, CancellationToken cancellationToken) {
        // VO factories validate here; invalid input throws DomainException -> 400.
        var fund = Fund.Create(
            request.Name,
            Isin.Create(request.Isin),
            Currency.Create(request.Currency));

        await _repository.AddAsync(fund, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return fund.Id;
    }
}