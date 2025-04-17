﻿using MagicalKitties.Application.Models.GlobalSettings;

namespace MagicalKitties.Application.Repositories;

public interface IGlobalSettingsRepository
{
    Task<bool> CreateSetting(GlobalSetting setting, CancellationToken token = default);
    Task<GlobalSetting?> GetSetting(string name, CancellationToken token = default);
    Task<IEnumerable<GlobalSetting>> GetAllAsync(GetAllGlobalSettingsOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(string name, CancellationToken token = default);
}