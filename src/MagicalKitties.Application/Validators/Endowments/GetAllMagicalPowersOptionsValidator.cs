using FluentValidation;
using MagicalKitties.Application.Models.MagicalPowers;

namespace MagicalKitties.Application.Validators.Endowments;

public class GetAllMagicalPowersOptionsValidator : AbstractValidator<GetAllMagicalPowersOptions>
{
    public GetAllMagicalPowersOptionsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 25).WithMessage("You can get between 1 and 25 characters per page");
    }
}