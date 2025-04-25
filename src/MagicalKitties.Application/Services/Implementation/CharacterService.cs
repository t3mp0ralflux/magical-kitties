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
    private readonly IValidator<GetAllCharactersOptions> _optionsValidator;

    public CharacterService(ICharacterRepository characterRepository, IValidator<Character> characterValidator, IValidator<GetAllCharactersOptions> optionsValidator)
    {
        _characterRepository = characterRepository;
        _characterValidator = characterValidator;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Character character, CancellationToken token = default)
    {
        await _characterValidator.ValidateAndThrowAsync(character, token);

        bool result = await _characterRepository.CreateAsync(character, token);

        return result;
    }

    public async Task<Character?> GetByIdAsync(Guid accountId, Guid id, CancellationToken token = default)
    {
        Character? result = await _characterRepository.GetByIdAsync(accountId, id, token: token);

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
}