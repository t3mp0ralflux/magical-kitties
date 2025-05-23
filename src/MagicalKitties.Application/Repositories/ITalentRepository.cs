﻿using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Repositories;

public interface ITalentRepository
{
    Task<bool> CreateAsync(Talent talent, CancellationToken token = default);
    Task<Talent?> GetByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<Talent>> GetAllAsync(GetAllTalentsOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(GetAllTalentsOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(Talent talent, CancellationToken token = default);
    Task<bool> DeleteAsync(int id, CancellationToken token = default);
}