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

                                           if (update.Update.Name?.Length > 100)
                                           {
                                               context.AddFailure(new ValidationFailure(nameof(update.Update.Name), "Name cannot be longer than 100 characters"));
                                           }

                                           break;
                                       case DescriptionOption.description:
                                           if (update.Update.Description?.Length > 250)
                                           {
                                               context.AddFailure(new ValidationFailure(nameof(update.Update.Description), "Description cannot be longer than 250 characters"));
                                           }
                                           break;
                                       case DescriptionOption.hometown:
                                           if (update.Update.Hometown?.Length > 100)
                                           {
                                               context.AddFailure(new ValidationFailure(nameof(update.Update.Hometown), "Hometown cannot be longer than 100 characters"));
                                           }
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