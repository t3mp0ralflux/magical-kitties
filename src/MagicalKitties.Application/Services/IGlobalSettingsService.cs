﻿using MagicalKitties.Application.Models.GlobalSettings;

namespace MagicalKitties.Application.Services;

public interface IGlobalSettingsService
{
    Task<bool> CreateSettingAsync(GlobalSetting setting, CancellationToken token = default);
    Task<IEnumerable<GlobalSetting>> GetAllAsync(GetAllGlobalSettingsOptions options, CancellationToken token = default);
    Task<int> GetCountAsync(string name, CancellationToken token = default);
    Task<GlobalSetting?> GetSettingAsync(string name, CancellationToken token = default);
    Task<T?> GetSettingAsync<T>(string name, T? defaultValue, CancellationToken token = default);
    Task<T?> GetSettingCachedAsync<T>(string name, T? defaultValue, CancellationToken token = default);
}