using FluentValidation;
using PAS.Application.Queries.Funds.GetFund;

namespace PAS.Application.Commands.CreateFund;

public sealed class GetFundListQueryValidator : AbstractValidator<GetFundQuery> {
    public GetFundListQueryValidator() {
    }
}