using FluentValidation;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Repositories;

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

    public async Task<Character> CopyAsync(Account account, Guid id, CancellationToken token = default)
    {
        Character? existingCharacter = await _characterRepository.GetByIdAsync(account.Id, id, false, token);

        Character copiedCharacter = existingCharacter!.CreateCopy();

        await _characterRepository.CopyAsync(copiedCharacter, token);

        return copiedCharacter;
    }

    public Task<bool> ExistsByIdAsync(Guid characterId, CancellationToken token = default)
    {
        return _characterRepository.ExistsByIdAsync(characterId, token);
    }

    public async Task<Character?> GetByIdAsync(Guid accountId, Guid id, CancellationToken token = default)
    {
        Character? result = await _characterRepository.GetByIdAsync(accountId, id, cancellationToken: token);

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