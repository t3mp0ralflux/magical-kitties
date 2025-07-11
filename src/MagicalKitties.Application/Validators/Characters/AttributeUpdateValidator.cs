using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Validators.Characters;

public class AttributeUpdateValidator : AbstractValidator<AttributeUpdateValidationContext>
{
    public AttributeUpdateValidator()
    {

        RuleFor(x => x.Update.Character)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Update.Cute)
            .Custom(ValidateAttribute)
            .When(x => x.Option == AttributeOption.cute);

        RuleFor(x => x.Update.Cunning)
            .Custom(ValidateAttribute)
            .When(x => x.Option == AttributeOption.cunning);

        RuleFor(x => x.Update.Fierce)
            .Custom(ValidateAttribute)
            .When(x => x.Option == AttributeOption.fierce);

        RuleFor(x => x.Update.Level)
            .NotNull()
            .InclusiveBetween(1, 10)
            .WithMessage("Level can only be between 1 and 10 inclusively.")
            .When(x => x.Option == AttributeOption.level);

        RuleFor(x => x.Update.CurrentOwies)
            .NotNull()
            .InclusiveBetween(0, 5)
            .WithMessage("Owies can only be between 0 and 5 inclusively.")
            .When(x => x.Option == AttributeOption.currentowies);

        RuleFor(x => x.Update.CurrentTreats)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Current Treats can't be negative.")
            .When(x => x.Option == AttributeOption.currenttreats);

        RuleFor(x => x.Update.CurrentInjuries)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Current Injuries can't be negative.")
            .When(x => x.Option == AttributeOption.currentinjuries);

        RuleFor(x => x.Update.FlawChange)
            .Custom(ValidateEndowment)
            .When(x => x.Option == AttributeOption.flaw);

        RuleFor(x => x.Update.TalentChange)
            .Custom(ValidateEndowment)
            .When(x => x.Option == AttributeOption.talent);

        RuleFor(x => x.Update.MagicalPowerChange)
            .Custom(ValidateEndowment)
            .When(x => x.Option == AttributeOption.magicalpower);
    }

    private static void ValidateAttribute(int? value, ValidationContext<AttributeUpdateValidationContext> context)
    {
        string fieldName = context.InstanceToValidate.Option switch
        {
            AttributeOption.cunning => nameof(AttributeUpdate.Cunning),
            AttributeOption.cute => nameof(AttributeUpdate.Cute),
            AttributeOption.fierce => nameof(AttributeUpdate.Fierce),
            _ => string.Empty
        };

        switch (value)
        {
            case null:
                context.AddFailure(new ValidationFailure(fieldName, $"'Update {fieldName}' must not be empty."));
                return;
            case < 0 or > 3:
                context.AddFailure(new ValidationFailure(fieldName, "Attribute can only be between 0 and 3 inclusively."));
                return;
        }

        Dictionary<AttributeOption, int> stats = new()
                                                 {
                                                     { AttributeOption.cunning, context.InstanceToValidate.Character.Cunning },
                                                     { AttributeOption.cute, context.InstanceToValidate.Character.Cute },
                                                     { AttributeOption.fierce, context.InstanceToValidate.Character.Fierce }
                                                 };

        if (value.Value == 0)
        {
            return; // don't care if they're all zeroes
        }

        // this is in case someone uses the API instead of the UI to change their information. Base values can only be 1,2,3 uniquely.
        // Upgrades are handled in the upgrade section and shown on the UI, NOT HERE.
        if (stats
            .Where(stat => stat.Key != context.InstanceToValidate.Option) // not the same option
            .Where(stat => stat.Value != 0) // not zero. don't care about three zeroes 
            .Any(stat => stat.Value == value.Value)) // care 'cause it's a match
        {
            context.AddFailure(new ValidationFailure(fieldName, "Another attribute is assigned that value. Reduce that first and try again."));
        }
    }

    private static void ValidateEndowment(EndowmentChange? value, ValidationContext<AttributeUpdateValidationContext> context)
    {
        string fieldName = context.InstanceToValidate.Option switch
        {
            AttributeOption.flaw => nameof(AttributeUpdate.FlawChange),
            AttributeOption.talent => nameof(AttributeUpdate.TalentChange),
            AttributeOption.magicalpower => nameof(AttributeUpdate.MagicalPowerChange),
            _ => string.Empty
        };

        switch (context.InstanceToValidate.Option)
        {
            case AttributeOption.flaw:
                if (value is null)
                {
                    context.AddFailure(new ValidationFailure(fieldName, $"'Update {fieldName}' cannot be null."));
                    return;
                }

                if (IdRangeIsInvalid(value.PreviousId))
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.PreviousId", $"Value of {fieldName} PreviousId was out of range."));
                    return;
                }

                if (IdRangeIsInvalid(value.NewId))
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", $"Value of {fieldName} NewId was out of range."));
                }

                break;
            case AttributeOption.talent:
                if (value is null)
                {
                    context.AddFailure(new ValidationFailure(fieldName, $"'Update {fieldName}' cannot be null."));
                    return;
                }

                if (IdRangeIsInvalid(value.NewId))
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", $"Value of {fieldName} NewId was out of range."));
                    return;
                }

                if (IdRangeIsInvalid(value.PreviousId))
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.PreviousId", $"Value of {fieldName} PreviousId was out of range."));
                    return;
                }

                // if it's empty, nothing else to see here
                if (context.InstanceToValidate.Character.Talents.Count == 0)
                {
                    return;
                }

                // duplicate
                if (context.InstanceToValidate.Character.Talents.FirstOrDefault(x => x.Id == value.NewId) is not null)
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", $"Talent '{value.NewId}' already present on character. Choose another."));
                    return;
                }

                bool previousTalentExists = context.InstanceToValidate.Character.Talents.FirstOrDefault(x => x.Id == value.PreviousId) is not null;

                // adding new one against restrictions
                if (context.InstanceToValidate.Character.Talents.Count == 2 && !previousTalentExists)
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", "Characters cannot have more than two Talents."));
                    return;
                }

                if (context.InstanceToValidate.Character.Talents.Count == 1 && context.InstanceToValidate.Character.Level < 5 && !previousTalentExists)
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", "Character is not level 5 or above. Cannot add new Talent."));
                }

                return;
            case AttributeOption.magicalpower:
                if (value is null)
                {
                    context.AddFailure(new ValidationFailure(fieldName, $"'Update {fieldName}' cannot be null."));
                    return;
                }

                if (IdRangeIsInvalid(value.NewId))
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", $"Value of {fieldName} NewId was out of range."));
                    return;
                }

                if (IdRangeIsInvalid(value.PreviousId))
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.PreviousId", $"Value of {fieldName} PreviousId was out of range."));
                    return;
                }

                // if it's empty, nothing else to see here
                if (context.InstanceToValidate.Character.MagicalPowers.Count == 0)
                {
                    return;
                }

                // duplicates
                if (context.InstanceToValidate.Character.MagicalPowers.FirstOrDefault(x => x.Id == value.NewId) is not null
                    || context.InstanceToValidate.Character.MagicalPowers.Any(x => x.BonusFeatures.FirstOrDefault(y => y.Id == value.NewId) is not null))
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", $"Magical Power '{value.NewId}' already present on character. Choose another."));
                    return;
                }

                bool previousMagicalPowerExists = context.InstanceToValidate.Character.MagicalPowers.FirstOrDefault(x => x.Id == value.PreviousId) is not null
                                                  || context.InstanceToValidate.Character.MagicalPowers.Any(x => x.BonusFeatures.FirstOrDefault(y => y.Id == value.PreviousId) is not null);

                // this is a special one. Undead can have a magical power as their bonus feature, which puts a wrench in the whole lvl 8+ can have two magical powers as they can have this at level 2.
                bool isUndead = context.InstanceToValidate.Character.MagicalPowers.FirstOrDefault(x => x.Id == 65) is not null;

                // adding new one against restrictions
                if (context.InstanceToValidate.Character.MagicalPowers.Count == 1 && context.InstanceToValidate.Character.Level < 8 && !previousMagicalPowerExists && !isUndead)
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", "Character is not level 8 or above or Undead. Cannot add new Magical Power."));
                    return;
                }

                if (context.InstanceToValidate.Character.MagicalPowers.Count == 2 && !previousMagicalPowerExists && !isUndead)
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", "Characters cannot have more than two Magical Powers."));
                    return;
                }

                // only for Undead if they try to add a fourth magical power.
                if (context.InstanceToValidate.Character.MagicalPowers.Count == 3 && !previousMagicalPowerExists)
                {
                    context.AddFailure(new ValidationFailure($"{fieldName}.NewId", "Even the Undead cannot have four Magical Powers."));
                }

                return;
            case AttributeOption.cunning:
            case AttributeOption.cute:
            case AttributeOption.fierce:
            case AttributeOption.level:
            case AttributeOption.currentowies:
            case AttributeOption.currenttreats:
            case AttributeOption.currentinjuries:
            default:
                return; // don't care, not us.
        }

        return;

        bool IdRangeIsInvalid(int id)
        {
            return id is
                (> 16 or < 11) and
                (> 26 or < 21) and
                (> 36 or < 31) and
                (> 46 or < 41) and
                (> 56 or < 51) and
                (> 66 or < 61);
        }
    }
}

public class AttributeUpdateValidationContext
{
    public required AttributeOption Option { get; init; }
    public required AttributeUpdate Update { get; init; }
    public required Character Character { get; init; }
}