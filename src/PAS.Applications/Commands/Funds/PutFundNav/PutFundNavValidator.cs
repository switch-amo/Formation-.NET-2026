using FluentValidation;
using PAS.Application.Commands.Funds.PutFundNav;

namespace PAS.Application.Commands.CreateFund;

public sealed class PutFundNavCommandValidator : AbstractValidator<PutFundNavCommand> {
    public PutFundNavCommandValidator() {
        RuleFor(x => x.Value)
            .NotNull()
            .GreaterThan(0);
    }
}