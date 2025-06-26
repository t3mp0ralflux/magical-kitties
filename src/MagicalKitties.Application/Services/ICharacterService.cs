using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Services;

public interface ICharacterService
{
    Task<bool> CreateAsync(Character character, CancellationToken token = default);
    Task<Character?> CopyAsync(Account account, Guid id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid characterId, CancellationToken token = default);
    Task<Character?> GetByIdAsync(Guid accountId, Guid id, CancellationToken token = default);
    Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
}