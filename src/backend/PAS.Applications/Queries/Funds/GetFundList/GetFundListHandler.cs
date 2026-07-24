using MediatR;
using PAS.Application.Dtos;
using PAS.Domain.Repositories;

namespace PAS.Application.Queries.Funds.GetFundList;

public sealed class GetFundListHandler : IRequestHandler<GetFundListQuery, IReadOnlyList<FundListItemDto>> {
    private readonly IFundRepository _repository;

    public GetFundListHandler(IFundRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<FundListItemDto>> Handle(GetFundListQuery request, CancellationToken cancellationToken) {
        var funds = await _repository.GetAllAsync(cancellationToken);

        return funds.Select(fund => new FundListItemDto(
                fund.Id,
                fund.Name,
                fund.Isin.Value,
                fund.Currency.Code,
                fund.Status.ToString(),
                fund.Navs.OrderByDescending(n => n.Date)
                    .Select(n => new NavDto(n.Date, n.Value))
                    .ToList()))
            .ToList();
    }
}