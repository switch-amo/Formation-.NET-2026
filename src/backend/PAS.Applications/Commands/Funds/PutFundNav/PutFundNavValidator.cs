using FluentValidation;

namespace PAS.Application.Commands.Funds.PutFundNav;

public sealed class PutFundNavCommandValidator : AbstractValidator<PutFundNavCommand> {
    public PutFundNavCommandValidator() {
        RuleFor(x => x.Value)
            .NotNull()
            .GreaterThan(0);
    }
}