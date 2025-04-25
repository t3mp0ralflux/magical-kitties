using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Validators.Characters;

public class DescriptionUpdateValidator: AbstractValidator<DescriptionUpdate>
{
    public DescriptionUpdateValidator()
    {
        RuleFor(x => x).Custom((update, context) =>
                               {
                                   switch (update.DescriptionOption)
                                   {
                                       case DescriptionOption.name:
                                           if (string.IsNullOrWhiteSpace(update.Name))
                                           {
                                               context.AddFailure(new ValidationFailure(nameof(update.Name), "Name cannot be empty"));
                                           }

                                           break;
                                       case DescriptionOption.xp:
                                           if (!update.XP.HasValue)
                                           {
                                               context.AddFailure(new ValidationFailure(nameof(update.XP), "XP cannot be empty"));
                                               break;
                                           }

                                           switch (update.XP.Value)
                                           {
                                               case < 0:
                                                   context.AddFailure(new ValidationFailure(nameof(update.XP), "XP cannot be negative"));
                                                   break;
                                               case > 100:
                                                   context.AddFailure(new ValidationFailure(nameof(update.XP), "XP value exceeds game capacity"));
                                                   break;
                                           }

                                           break;
                                       case DescriptionOption.description:
                                       case DescriptionOption.hometown:
                                           // these can be empty, sure. leave em.
                                           break;
                                       default:
                                           throw new ArgumentOutOfRangeException();
                                   }
                               });
    }
}