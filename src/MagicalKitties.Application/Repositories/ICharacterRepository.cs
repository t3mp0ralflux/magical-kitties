using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Repositories;

public interface ICharacterRepository
{
    Task<bool> CreateAsync(Character character, CancellationToken token = default);
    Task<Character?> GetByIdAsync(Guid characterId, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<bool?> ExistsByIdAsync(Guid accountId, Guid characterId, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid accountId, Guid characterId, CancellationToken token = default);
    Task<bool> CopyAsync(Character existingCharacter, CancellationToken token = default);
}