using FluentValidation;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Validators.Endowments;

public class GetAllTalentsOptionsValidator : AbstractValidator<GetAllTalentsOptions>
{
    public GetAllTalentsOptionsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 25).WithMessage("You can get between 1 and 25 characters per page");
    }
}