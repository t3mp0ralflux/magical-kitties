using FluentValidation;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Validators.Characters;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterUpdateService : ICharacterUpdateService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ICharacterUpdateRepository _characterUpdateRepository;
    private readonly IValidator<DescriptionUpdate> _descriptionUpdateValidator;
    private readonly IValidator<AttributeUpdateValidationContext> _attributeUpdateValidator;

    public CharacterUpdateService(ICharacterRepository characterRepository, ICharacterUpdateRepository characterUpdateRepository, IValidator<DescriptionUpdate> descriptionUpdateValidator, IValidator<AttributeUpdateValidationContext> attributeUpdateValidator)
    {
        _characterRepository = characterRepository;
        _characterUpdateRepository = characterUpdateRepository;
        _descriptionUpdateValidator = descriptionUpdateValidator;
        _attributeUpdateValidator = attributeUpdateValidator;
    }

    public async Task<bool> UpdateDescriptionAsync(DescriptionUpdate update, CancellationToken token = default)
    {
        bool characterExists = await _characterRepository.ExistsByIdAsync(update.CharacterId, token);

        if (!characterExists)
        {
            return false;
        }
        
        await _descriptionUpdateValidator.ValidateAndThrowAsync(update, token);

        return update.DescriptionOption switch
        {
            DescriptionOption.name => await _characterUpdateRepository.UpdateNameAsync(update, token), // validated above. won't be null here.
            DescriptionOption.description => await _characterUpdateRepository.UpdateDescriptionAsync(update, token),
            DescriptionOption.hometown => await _characterUpdateRepository.UpdateHometownAsync(update, token),
            DescriptionOption.xp => await _characterUpdateRepository.UpdateXPAsync(update, token),
            _ => throw new ValidationException("Selected description option not valid")
        };
    }

    public async Task<bool> UpdateAttributeAsync(AttributeUpdate update, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(update.AccountId, update.CharacterId, cancellationToken: token);

        if (character is null)
        {
            return false;
        }

        AttributeUpdateValidationContext validationContext = new AttributeUpdateValidationContext
                                                             {
                                                                 Character = character,
                                                                 Update = update
                                                             };
        
        // also validates if someone has put two threes or two ones for some reason.
        await _attributeUpdateValidator.ValidateAndThrowAsync(validationContext, token);
        
        switch (update.AttributeOption)
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

                return await _characterUpdateRepository.UpdateOwiesAsync(update, token);
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
            default:
                throw new ValidationException("Selected attribute option not valid");
        }
    }
}