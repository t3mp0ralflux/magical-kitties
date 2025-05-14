using FluentValidation;
using MagicalKitties.Application.Models.Flaws;

namespace MagicalKitties.Application.Validators.Endowments;

public class FlawValidator : AbstractValidator<Flaw>
{
    public FlawValidator()
    {
        RuleFor(x => x.Id).Custom(ValidateIdRange);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
    }

    public static void ValidateIdRange(int id, ValidationContext<Flaw> context)
    {
        switch (id)
        {
            case >= 11 and <= 16:
            case >= 21 and <= 26:
            case >= 31 and <= 36:
            case >= 41 and <= 46:
            case >= 51 and <= 56:
            case >= 61 and <= 66:
                return;
            default:
                context.AddFailure("Flaw entry must have a number 1-6 in the tens and 1-6 in the ones. Eg. 11,41,66, etc");
                break;
        }
    }
}