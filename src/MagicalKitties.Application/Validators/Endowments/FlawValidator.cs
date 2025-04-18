using FluentValidation;
using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Validators.Endowments;

public class FlawValidator : AbstractValidator<Endowment>
{
    public FlawValidator()
    {
        RuleFor(x => x.Id)
            .InclusiveBetween(11, 16)
            .InclusiveBetween(21, 26)
            .InclusiveBetween(31, 36)
            .InclusiveBetween(41, 46)
            .InclusiveBetween(51, 56)
            .InclusiveBetween(61, 66)
            .WithMessage("Flaw entry must have a number 1-6 in the tens and 1-6 in the ones. Eg. 11,41,66, etc");

        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
    }
}