using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Services;

public interface ICharacterService
{
    /// <summary>
    /// Create a new Kitty
    /// </summary>
    /// <param name="character">Character to create</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    Task<bool> CreateAsync(Character character, CancellationToken token = default);
    Task<Character> CopyAsync(Guid characterId, CancellationToken token = default);
    
    /// <summary>
    /// Verify if the character exists for this account.
    /// </summary>
    /// <param name="accountId">Account Id for the Character</param>
    /// <param name="characterId">Id of the Character</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>True if the character exists, false if it doesn't, null if it doesn't belong to this account.</returns>
    Task<bool?> ExistsByIdAsync(Guid accountId, Guid characterId, CancellationToken token = default);
    Task<Character?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid characterId, Guid humanId, CancellationToken token = default);
}