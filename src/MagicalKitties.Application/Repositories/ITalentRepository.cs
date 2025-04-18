﻿using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Repositories;

public interface ITalentRepository
{
    Task<bool> CreateAsync(Endowment talent, CancellationToken token = default);
    Task<Endowment?> GetByIdAsync(int id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<Endowment>> GetAllAsync(GetAllTalentsOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllTalentsOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(Endowment talent, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}