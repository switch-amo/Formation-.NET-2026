using MediatR;
using PAS.Domain.Repositories;

namespace PAS.Application.Queries.Funds.GetFund;

public sealed class GetFundHandler : IRequestHandler<GetFundQuery, FundDto?> {
    private readonly IFundRepository _repository;

    public GetFundHandler(IFundRepository repository) => _repository = repository;

    public async Task<FundDto?> Handle(GetFundQuery request, CancellationToken cancellationToken) {
        var fund = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (fund is null) return null;

        return new FundDto(
            fund.Id,
            fund.Name,
            fund.Isin.Value,
            fund.Currency.Code,
            fund.Status.ToString(),
            fund.Navs
                .OrderByDescending(n => n.Date)
                .Select(n => new NavDto(n.Date, n.Value))
                .ToList());
    }
}