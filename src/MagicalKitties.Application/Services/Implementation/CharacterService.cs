using FluentValidation;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IValidator<Character> _characterValidator;
    private readonly IFlawRepository _flawRepository;
    private readonly ILogger<CharacterService> _logger;
    private readonly IValidator<GetAllCharactersOptions> _optionsValidator;
    private readonly ITalentRepository _talentRepository;

    public CharacterService(ICharacterRepository characterRepository, IValidator<Character> characterValidator, IValidator<GetAllCharactersOptions> optionsValidator, ILogger<CharacterService> logger, IFlawRepository flawRepository, ITalentRepository talentRepository)
    {
        _characterRepository = characterRepository;
        _characterValidator = characterValidator;
        _optionsValidator = optionsValidator;
        _logger = logger;
        _flawRepository = flawRepository;
        _talentRepository = talentRepository;
    }

    public async Task<bool> CreateAsync(Character character, CancellationToken token = default)
    {
        await _characterValidator.ValidateAndThrowAsync(character, token);

        bool result = await _characterRepository.CreateAsync(character, token);

        return result;
    }

    public async Task<Character?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        Character? result = await _characterRepository.GetByIdAsync(id, token: token);

        return result;
    }

    public async Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);

        return await _characterRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        return await _characterRepository.GetCountAsync(options, token);
    }

    public async Task<bool> UpdateAsync(Character character, CancellationToken token = default)
    {
        bool exists = await _characterRepository.ExistsByIdAsync(character.Id, token);

        if (!exists)
        {
            return false; // not found
        }

        return await _characterRepository.UpdateAsync(character, token);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        bool exists = await _characterRepository.ExistsByIdAsync(id, token);

        if (!exists)
        {
            return false; // not found
        }

        return await _characterRepository.DeleteAsync(id, token);
    }

    public async Task<bool> UpdateLevelAsync(LevelUpdate update, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(update.CharacterId, token: token);

        if (character is null)
        {
            return false;
        }

        return await _characterRepository.UpdateLevelAsync(update, token);
    }

    public async Task<bool> UpdateFlawAsync(FlawUpdate update, CancellationToken token = default)
    {
        bool flawExists = await _flawRepository.ExistsByIdAsync(update.FlawId, token);

        if (!flawExists)
        {
            throw new ValidationException("Flaw not found");
        }

        bool characterExists = await _characterRepository.ExistsByIdAsync(update.CharacterId, token);

        if (!characterExists)
        {
            return false;
        }

        return await _characterRepository.UpdateFlawAsync(update, token);
    }

    public async Task<bool> UpdateTalentAsync(TalentUpdate update, CancellationToken token = default)
    {
        bool flawExists = await _talentRepository.ExistsByIdAsync(update.TalentId, token);

        if (!flawExists)
        {
            throw new ValidationException("Flaw not found");
        }

        bool characterExists = await _characterRepository.ExistsByIdAsync(update.CharacterId, token);

        if (!characterExists)
        {
            return false;
        }

        return await _characterRepository.UpdateTalentAsync(update, token);
    }
}