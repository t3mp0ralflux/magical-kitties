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

        // TODO: Update this to handle MagicalPowers so it doesn't suck. Needs to handle a Sub magical power update.
        return update.AttributeOption switch
        {
            AttributeOption.cunning => await _characterUpdateRepository.UpdateCunningAsync(update, ApplicationAssumptions.CunningAttributeId, token),
            AttributeOption.cute => await _characterUpdateRepository.UpdateCuteAsync(update, ApplicationAssumptions.CuteAttributeId, token),
            AttributeOption.fierce => await _characterUpdateRepository.UpdateFierceAsync(update, ApplicationAssumptions.FierceAttributeId, token),
            AttributeOption.level => await _characterUpdateRepository.UpdateLevelAsync(update, token),
            AttributeOption.flaw => await _characterUpdateRepository.UpdateFlawAsync(update, token),
            AttributeOption.talent => await _characterUpdateRepository.UpdateTalentAsync(update, token),
            AttributeOption.magicalpower => await _characterUpdateRepository.UpdateMagicalPowerAsync(update, token),
            AttributeOption.owies => await _characterUpdateRepository.UpdateOwiesAsync(update, token),
            AttributeOption.currenttreats => await _characterUpdateRepository.UpdateCurrentTreatsAsync(update, token),
            _ => throw new ValidationException("Selected attribute option not valid")
        };
    }
}