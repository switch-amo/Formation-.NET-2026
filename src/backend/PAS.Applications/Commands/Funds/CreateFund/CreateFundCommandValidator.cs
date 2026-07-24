using FluentValidation;

namespace PAS.Application.Commands.Funds.CreateFund;

public sealed class CreateFundCommandValidator : AbstractValidator<CreateFundCommand> {
    public CreateFundCommandValidator() {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Isin)
            .NotEmpty()
            .Length(12);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3);
    }
}