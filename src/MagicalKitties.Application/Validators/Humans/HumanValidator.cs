using FluentValidation;
using MagicalKitties.Application.Models.Humans;

namespace MagicalKitties.Application.Validators.Humans;

public class HumanValidator : AbstractValidator<Human>
{
    public HumanValidator()
    {
        RuleFor(x => x.CharacterId)
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}