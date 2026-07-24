using MediatR;
using PAS.Application.Abstractions;
using PAS.Domain.Repositories;

namespace PAS.Application.Commands.Funds.PutFundNav;

public sealed class PutFundNavHandler : IRequestHandler<PutFundNavCommand> {
    private readonly IFundRepository _repository;
    private readonly TimeProvider _timeProvider;

    public PutFundNavHandler(IFundRepository repository, TimeProvider timeProvider) {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task Handle(PutFundNavCommand request, CancellationToken cancellationToken) {
        var fund = await _repository.GetByIdAsync(request.FundId, cancellationToken)
            ?? throw new NotFoundException($"Fund '{request.FundId}' was not found");

        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        fund.AddNav(request.Date, request.Value, today);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}