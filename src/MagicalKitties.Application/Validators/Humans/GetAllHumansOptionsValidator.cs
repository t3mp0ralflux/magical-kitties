using FluentValidation;
using MagicalKitties.Application.Models.Humans;

namespace MagicalKitties.Application.Validators.Humans;

public class GetAllHumansOptionsValidator : AbstractValidator<GetAllHumansOptions>
{
    public static readonly string[] AcceptableSortFields =
    [
        "id",
        "name"
    ];
    
    public GetAllHumansOptionsValidator()
    {
        RuleFor(x => x.CharacterId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 10).WithMessage("You can get between 1 and 10 humans per page");
        RuleFor(x => x.SortField).Must(x => x is null || AcceptableSortFields.Contains(x)).WithMessage("You can only sort by id or name");
    }
}