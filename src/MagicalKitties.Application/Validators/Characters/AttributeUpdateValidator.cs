using System.Data;
using System.Security.Cryptography;
using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Validators.Characters;

public class AttributeUpdateValidator : AbstractValidator<AttributeUpdateValidationContext>
{
    public AttributeUpdateValidator()
    {
        RuleFor(x => x.Update.AccountId)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Update.CharacterId)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Update.Cute)
            .Custom(ValidateAttribute)
            .When(x => x.Update.AttributeOption == AttributeOption.cute);
        
        RuleFor(x => x.Update.Cunning)
            .Custom(ValidateAttribute)
            .When(x => x.Update.AttributeOption == AttributeOption.cunning);
        
        RuleFor(x => x.Update.Fierce)
            .Custom(ValidateAttribute)
            .When(x => x.Update.AttributeOption == AttributeOption.fierce);
        
        RuleFor(x => x.Update.Level)
            .NotNull()
            .InclusiveBetween(1,10)
            .WithMessage("Level can only be between 1 and 10 inclusively")
            .When(x => x.Update.AttributeOption == AttributeOption.level);

        RuleFor(x => x.Update.Owies)
            .NotNull()
            .InclusiveBetween(0, 5)
            .WithMessage("Owies can only be between 0 and 5 inclusively")
            .When(x=>x.Update.AttributeOption == AttributeOption.owies);
        
        RuleFor(x => x.Update.CurrentTreats)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Current treats can't be negative.")
            .When(x=>x.Update.AttributeOption == AttributeOption.currenttreats);
        
        RuleFor(x => x.Update.FlawChange)
            .Custom(ValidateEndowment)
            .When(x=>x.Update.AttributeOption == AttributeOption.flaw);
        
        RuleFor(x => x.Update.TalentChange)
            .Custom(ValidateEndowment)
            .When(x=>x.Update.AttributeOption == AttributeOption.talent);
        
        RuleFor(x => x.Update.MagicalPowerChange)
            .Custom(ValidateEndowment)
            .When(x=>x.Update.AttributeOption == AttributeOption.magicalpower);
        
    }

    private static void ValidateAttribute(int? value, ValidationContext<AttributeUpdateValidationContext> context)
    {
        string fieldName = context.InstanceToValidate.Update.AttributeOption.ToString();
        switch (value)
        {
            case null:
                context.AddFailure(new ValidationFailure(fieldName, "Attribute must have a value"));
                return;
            case < 0 or > 3:
                context.AddFailure(new ValidationFailure(fieldName, "Attributes cannot go below 0 or above 3"));
                return;
        }
        
        Dictionary<AttributeOption, int> stats = new Dictionary<AttributeOption, int>()
                                                 {
                                                     {AttributeOption.cunning, context.InstanceToValidate.Character.Cunning },
                                                     {AttributeOption.cute, context.InstanceToValidate.Character.Cute },
                                                     {AttributeOption.fierce, context.InstanceToValidate.Character.Fierce },
                                                 };
        
        if (value.Value == 0)
        {
            return; // don't care if they're all zeroes
        }

        // this is in case someone uses the API instead of the UI to change their information. Base values can only be 1,2,3 uniquely.
        // Upgrades are handled in the upgrade section and shown on the UI, NOT HERE.
        if (stats
            .Where(stat => stat.Key != context.InstanceToValidate.Update.AttributeOption) // not the same option
            .Where(stat => stat.Value != 0) // not zero. don't care about three zeroes 
            .Any(stat => stat.Value == value.Value)) // care cause it's a match
        {
            context.AddFailure(new ValidationFailure(fieldName, "Another attribute is assigned that value. Reduce that first and try again"));
        }
    }

    private static void ValidateEndowment(EndowmentChange? value, ValidationContext<AttributeUpdateValidationContext> context)
    {
        switch (context.InstanceToValidate.Update.AttributeOption)
        {
            case AttributeOption.flaw:
                if (value is null)
                {
                    context.AddFailure(new ValidationFailure(context.InstanceToValidate.Update.AttributeOption.ToString(), "Value cannot be null"));
                    return;
                }

                if (IdRangeIsInvalid(value.PreviousId))
                {
                    context.AddFailure(new ValidationFailure("FlawChange.PreviousId", "Value of previous Id was out range"));
                    return;
                }
                
                if (IdRangeIsInvalid(value.NewId))
                {
                    context.AddFailure(new ValidationFailure("FlawChange.NewId", "Value of new Id was out range"));
                }
                break;
            case AttributeOption.talent:
                if (value is null)
                {
                    context.AddFailure(new ValidationFailure(context.InstanceToValidate.Update.AttributeOption.ToString(), "Value cannot be null"));
                    return;
                }
                
                if (IdRangeIsInvalid(value.NewId))
                {
                    context.AddFailure(new ValidationFailure("TalentChange.NewId", "Value of new Id was out range"));
                    return;
                }
                
                // if it's empty, nothing else to see here
                if (context.InstanceToValidate.Character.Talents.Count == 0)
                {
                    return; 
                }
                
                if (IdRangeIsInvalid(value.PreviousId))
                {
                    context.AddFailure(new ValidationFailure("TalentChange.PreviousId", "Value of previous Id was out range"));
                    return;
                }

                if (context.InstanceToValidate.Character.Talents.FirstOrDefault(x => x.Id == value.NewId) is not null)
                {
                    context.AddFailure(new ValidationFailure("TalentChange.NewId", "Talent already present on character. Choose another"));
                    return;
                }
                
                if (context.InstanceToValidate.Character.Talents.FirstOrDefault(x => x.Id == value.PreviousId) is null)
                {
                    context.AddFailure(new ValidationFailure("TalentChange.PreviousId", "Talent not present on character. Choose another"));
                }
                
                return;
            case AttributeOption.magicalpower:
                if (value is null)
                {
                    context.AddFailure(new ValidationFailure(context.InstanceToValidate.Update.AttributeOption.ToString(), "Value cannot be null"));
                    return;
                }

                if (IdRangeIsInvalid(value.NewId))
                {
                    context.AddFailure(new ValidationFailure("PreviousId", "Value of previous Id was out range."));
                    return;
                }
                
                // if it's empty, nothing else to see here
                if (context.InstanceToValidate.Character.MagicalPowers.Count == 0)
                {
                    return; 
                }
                
                if (IdRangeIsInvalid(value.PreviousId))
                {
                    context.AddFailure(new ValidationFailure("PreviousId", "Value of previous Id was out range."));
                    return;
                }
                
                if (context.InstanceToValidate.Character.MagicalPowers.FirstOrDefault(x => x.Id == value.PreviousId) is null)
                {
                    if (context.InstanceToValidate.Character.MagicalPowers.Any(x => x.BonusFeatures.FirstOrDefault(y => y.Id == value.PreviousId) is null))
                    {
                        context.AddFailure(new ValidationFailure("MagicalPower.PreviousId", "Magical Power not present on character. Choose another"));
                        return;
                    }
                }
                
                if (context.InstanceToValidate.Character.MagicalPowers.FirstOrDefault(x => x.Id == value.NewId) is not null 
                    || context.InstanceToValidate.Character.MagicalPowers.Any(x => x.BonusFeatures.FirstOrDefault(y => y.Id == value.NewId) is not null))
                {
                    context.AddFailure(new ValidationFailure("MagicalPower.NewId", "Magical Power already present on character. Choose another"));
                }

                break;
            case AttributeOption.cunning:
            case AttributeOption.cute:
            case AttributeOption.fierce:
            case AttributeOption.level:
            case AttributeOption.owies:
            case AttributeOption.currenttreats:
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
    public required AttributeUpdate Update { get; set; }
    public required Character Character { get; set; }
}