﻿using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Repositories;

public interface ICharacterRepository
{
    Task<bool> CreateAsync(Character character, CancellationToken token = default);
    Task<Character?> GetByIdAsync(Guid accountId, Guid id, bool includeDeleted = false, CancellationToken token = default);
    Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> UpdateAsync(Character character, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
}