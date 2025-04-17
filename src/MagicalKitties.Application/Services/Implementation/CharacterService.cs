using System.Collections;
using System.Xml.XPath;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Accounts;
using FluentValidation;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IValidator<Character> _characterValidator;
    private readonly IValidator<GetAllCharactersOptions> _optionsValidator;
    private readonly ILogger<CharacterService> _logger;

    public CharacterService(ICharacterRepository characterRepository, IValidator<Character> characterValidator, IValidator<GetAllCharactersOptions> optionsValidator, ILogger<CharacterService> logger)
    {
        _characterRepository = characterRepository;
        _characterValidator = characterValidator;
        _optionsValidator = optionsValidator;
        _logger = logger;
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
        
        IEnumerable<Character> results = await _characterRepository.GetAllAsync(options, token);

        return results;
    }

    public async Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        int result = await _characterRepository.GetCountAsync(options, token);

        return result;
    }

    public async Task<bool> UpdateAsync(Character character, CancellationToken token = default)
    {
        bool exists = await _characterRepository.ExistsByIdAsync(character.Id, token);

        if (!exists)
        {
            return false; // not found
        }

        bool result = await _characterRepository.UpdateAsync(character, token);

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        var exists = await _characterRepository.ExistsByIdAsync(id, token);

        if (!exists)
        {
            return false; // not found
        }

        bool result = await _characterRepository.DeleteAsync(id, token);

        return result;
    }
    
    public async Task<bool> ChangeLevelAsync(Guid characterId, int level, CancellationToken token = default)
    {
        // Character? character = await _characterRepository.GetByIdAsync(characterId, token: token);
        //
        // if (character is null)
        // {
        //     return false;
        // }
        //
        // LevelInfo? levelInfo = await _levelInfoRepository.GetByLevelAsync(level, token);
        //
        // if (levelInfo is null)
        // {
        //     _logger.LogError("Level {Level} not found in DB", level);
        //     throw new Exception("Error updating character level. Contact support for assistance.");
        // }
        //
        // if (level < character.Level)
        // {
        //     // doing down. Reset xp to base level.
        //     character.CurrentXp = 0;
        // }
        //
        // await _characterRepository.UpdateLevelAsync(character, levelInfo.Id, token);
        //
        // return true;

        return true; // TODO: DON'T LEAVE IT THIS WAY.
    }
}