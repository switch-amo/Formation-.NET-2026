using MediatR;
using PAS.Asset.Application.Abstractions;
using PAS.Domain.Repositories;

namespace PAS.Application.Commands.Funds.PutFundNav;

public sealed class PutFundNavHandler : IRequestHandler<PutFundNavCommand> {
    private readonly IFundRepository _repository;

    public PutFundNavHandler(IFundRepository repository) => _repository = repository;

    public async Task Handle(PutFundNavCommand request, CancellationToken cancellationToken) {
        var fund = await _repository.GetByIdAsync(request.FundId, cancellationToken)
            ?? throw new NotFoundException($"Fund '{request.FundId}' was not found");

        fund.AddNav(request.Date, request.Value);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}