using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Validators.Characters;

public class DescriptionUpdateValidator : AbstractValidator<DescriptionUpdateValidationContext>
{
    public DescriptionUpdateValidator()
    {
        RuleFor(x => x.Update.AccountId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Update.CharacterId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x).Custom((update, context) =>
                               {
                                   switch (update.Option)
                                   {
                                       case DescriptionOption.name:
                                           if (string.IsNullOrWhiteSpace(update.Update.Name))
                                           {
                                               context.AddFailure(new ValidationFailure(nameof(update.Update.Name), "Name cannot be empty"));
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

public class DescriptionUpdateValidationContext
{
    public required DescriptionOption Option { get; init; }
    public required DescriptionUpdate Update { get; init; }
}