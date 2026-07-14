using MediatR;
using PAS.Application.Dtos;
using PAS.Application.Repositories;

namespace PAS.Application.Queries.GetFundList;

public sealed class GetFundListHandler : IRequestHandler<GetFundListQuery, IReadOnlyCollection<FundDto>> {
    private readonly IFundRepository _fundRepository;

    public GetFundListHandler(IFundRepository fundRepository) {
        _fundRepository = fundRepository;
    }

    public async Task<IReadOnlyCollection<FundDto>> Handle(GetFundListQuery request, CancellationToken cancellationToken) {
        var funds = await _fundRepository.GetAllAsync(cancellationToken);

        return funds.Select(fund => new FundDto {
                Id = fund.Id,
                Name = fund.Name,
                Isin = fund.Isin,
                Currency = fund.Currency,
                Status = fund.Status.ToString()
            }).ToList();
    }
}