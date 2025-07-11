using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Validators.Characters;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterUpdateService : ICharacterUpdateService
{
    private readonly IValidator<AttributeUpdateValidationContext> _attributeUpdateValidator;
    private readonly ICharacterRepository _characterRepository;
    private readonly ICharacterUpdateRepository _characterUpdateRepository;
    private readonly IValidator<DescriptionUpdateValidationContext> _descriptionUpdateValidator;

    public CharacterUpdateService(ICharacterRepository characterRepository, ICharacterUpdateRepository characterUpdateRepository, IValidator<DescriptionUpdateValidationContext> descriptionUpdateValidator, IValidator<AttributeUpdateValidationContext> attributeUpdateValidator)
    {
        _characterRepository = characterRepository;
        _characterUpdateRepository = characterUpdateRepository;
        _descriptionUpdateValidator = descriptionUpdateValidator;
        _attributeUpdateValidator = attributeUpdateValidator;
    }

    public async Task<bool> UpdateDescriptionAsync(DescriptionOption option, DescriptionUpdate update, CancellationToken token = default)
    {
        DescriptionUpdateValidationContext validationContext = new()
                                                               {
                                                                   Option = option,
                                                                   Update = update
                                                               };

        await _descriptionUpdateValidator.ValidateAndThrowAsync(validationContext, token);

        return option switch
        {
            DescriptionOption.name => await _characterUpdateRepository.UpdateNameAsync(update, token), // validated above. won't be null here.
            DescriptionOption.description => await _characterUpdateRepository.UpdateDescriptionAsync(update, token),
            DescriptionOption.hometown => await _characterUpdateRepository.UpdateHometownAsync(update, token),
            DescriptionOption.xp => await _characterUpdateRepository.UpdateXPAsync(update, token),
            _ => throw new ValidationException([new ValidationFailure("DescriptionOption", "Selected description option not valid")])
        };
    }

    public async Task<bool> UpdateAttributeAsync(AttributeOption option, AttributeUpdate update, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(update.Character.Id, cancellationToken: token);

        if (character is null)
        {
            return false;
        }

        AttributeUpdateValidationContext validationContext = new()
                                                             {
                                                                 Option = option,
                                                                 Character = character,
                                                                 Update = update
                                                             };

        // also validates if someone has put two threes or two ones for some reason.
        await _attributeUpdateValidator.ValidateAndThrowAsync(validationContext, token);

        switch (option)
        {
            case AttributeOption.cunning:
                if (character.Cunning == update.Cunning)
                {
                    return true; // no need to update
                }

                return await _characterUpdateRepository.UpdateCunningAsync(update, token);
            case AttributeOption.cute:
                if (character.Cute == update.Cute)
                {
                    return true;
                }

                return await _characterUpdateRepository.UpdateCuteAsync(update, token);
            case AttributeOption.fierce:
                if (character.Fierce == update.Fierce)
                {
                    return true;
                }

                return await _characterUpdateRepository.UpdateFierceAsync(update, token);
            case AttributeOption.level:
                if (character.Level == update.Level)
                {
                    return true;
                }

                return await _characterUpdateRepository.UpdateLevelAsync(update, token);
            case AttributeOption.flaw:
                if (character.Flaw is null)
                {
                    return await _characterUpdateRepository.CreateFlawAsync(update, token);
                }

                if (character.Flaw.Id == update.FlawChange!.NewId)
                {
                    return true;
                }

                return await _characterUpdateRepository.UpdateFlawAsync(update, token);
            case AttributeOption.talent:
                if (character.Talents.Count == 0 || character.Talents.FirstOrDefault(x => x.Id == update.TalentChange!.PreviousId) is null)
                {
                    return await _characterUpdateRepository.CreateTalentAsync(update, token);
                }

                return await _characterUpdateRepository.UpdateTalentAsync(update, token);
            case AttributeOption.magicalpower:
                if (character.MagicalPowers.Count == 0 || character.MagicalPowers.FirstOrDefault(x => x.Id == update.MagicalPowerChange!.PreviousId) is null)
                {
                    return await _characterUpdateRepository.CreateMagicalPowerAsync(update, token);
                }

                return await _characterUpdateRepository.UpdateMagicalPowerAsync(update, token);
            case AttributeOption.currentowies:
                if (character.CurrentOwies == update.CurrentOwies)
                {
                    return true; // no need to update
                }

                return await _characterUpdateRepository.UpdateCurrentOwiesAsync(update, token);
            case AttributeOption.currenttreats:
                if (character.CurrentTreats == update.CurrentTreats)
                {
                    return true; // no need to update
                }

                return await _characterUpdateRepository.UpdateCurrentTreatsAsync(update, token);
            case AttributeOption.currentinjuries:
                if (character.CurrentInjuries == update.CurrentInjuries)
                {
                    return true; // no need to update
                }

                return await _characterUpdateRepository.UpdateCurrentInjuriesAsync(update, token);
            case AttributeOption.incapacitated:
                if (character.Incapacitated == update.Incapacitated)
                {
                    return true;
                }

                return await _characterUpdateRepository.UpdateIncapacitatedStatus(update, token);
            default:
                throw new ValidationException([new ValidationFailure("AttributeOption", "Selected attribute option not valid")]);
        }
    }

    public async Task<bool> Reset(Guid characterId, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(characterId, cancellationToken: token);

        if (character is null)
        {
            return false;
        }

        AttributeUpdate update = new()
                                 {
                                     Character = character,
                                     CurrentOwies = 0,
                                     CurrentInjuries = 0,
                                     CurrentTreats = character.StartingTreats,
                                     Incapacitated = false
                                 };

        // reset current owies, treats, injuries.
        await _characterUpdateRepository.UpdateCurrentOwiesAsync(update, token);

        await _characterUpdateRepository.UpdateCurrentInjuriesAsync(update, token);

        await _characterUpdateRepository.UpdateCurrentTreatsAsync(update, token);

        await _characterUpdateRepository.UpdateIncapacitatedStatus(update, token);

        return true;
    }
}