using FluentValidation;
using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Validators.Characters;

public class CharacterValidator : AbstractValidator<Character>
{
    public CharacterValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.AccountId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotNull().NotEmpty();
    }
}