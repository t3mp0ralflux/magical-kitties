using FluentValidation;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterUpdateService : ICharacterUpdateService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ICharacterUpdateRepository _characterUpdateRepository;
    private readonly IValidator<DescriptionUpdate> _descriptionUpdateValidator;

    public CharacterUpdateService(ICharacterRepository characterRepository, ICharacterUpdateRepository characterUpdateRepository, IValidator<DescriptionUpdate> descriptionUpdateValidator)
    {
        _characterRepository = characterRepository;
        _characterUpdateRepository = characterUpdateRepository;
        _descriptionUpdateValidator = descriptionUpdateValidator;
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
}