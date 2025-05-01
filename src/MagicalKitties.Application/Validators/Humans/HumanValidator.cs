using FluentValidation;
using MagicalKitties.Application.Models.Humans;

namespace MagicalKitties.Application.Validators.Humans;

public class HumanValidator : AbstractValidator<Human>
{
    public HumanValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .Equal(Guid.Empty);
        
        RuleFor(x => x.CharacterId)
            .NotNull()
            .Equal(Guid.Empty);

        RuleFor(x => x.Name)
            .NotNull();
        
        RuleFor(x => x.Description)
            .NotNull();

        RuleFor(x => x.Problems)
            .NotNull();
    }
}