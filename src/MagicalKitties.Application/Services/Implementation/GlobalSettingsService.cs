﻿using FluentValidation;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MagicalKitties.Application.Services.Implementation;

public class GlobalSettingsService : IGlobalSettingsService
{
    private readonly IMemoryCache _cache;
    private readonly IGlobalSettingsRepository _globalSettingsRepository;
    private readonly IValidator<GlobalSetting> _globalSettingValidator;
    private readonly IValidator<GetAllGlobalSettingsOptions> _optionsValidator;
    private readonly ILogger<GlobalSettingsService> _logger;

    public GlobalSettingsService(IGlobalSettingsRepository globalSettingsRepository, IValidator<GlobalSetting> globalSettingValidator, IValidator<GetAllGlobalSettingsOptions> optionsValidator, IMemoryCache cache, ILogger<GlobalSettingsService> logger)
    {
        _globalSettingsRepository = globalSettingsRepository;
        _globalSettingValidator = globalSettingValidator;
        _optionsValidator = optionsValidator;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> CreateSettingAsync(GlobalSetting setting, CancellationToken token = default)
    {
        await _globalSettingValidator.ValidateAndThrowAsync(setting, token);

        return await _globalSettingsRepository.CreateSetting(setting, token);
    }

    public async Task<IEnumerable<GlobalSetting>> GetAllAsync(GetAllGlobalSettingsOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);

        return await _globalSettingsRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(string name, CancellationToken token = default)
    {
        return await _globalSettingsRepository.GetCountAsync(name, token);
    }

    public async Task<GlobalSetting?> GetSettingAsync(string name, CancellationToken token = default)
    {
        GlobalSetting? setting = await _globalSettingsRepository.GetSetting(name, token);

        return setting;
    }

    public async Task<T?> GetSettingAsync<T>(string name, T? defaultValue, CancellationToken token = default)
    {
        GlobalSetting? setting = await _globalSettingsRepository.GetSetting(name, token);

        if (setting is null)
        {
            return defaultValue;
        }

        try
        {
            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogError("GlobalSetting conversion error for {SettingName}: {Error}", name, ex.Message);
            return defaultValue;
        }
    }

    public async Task<T?> GetSettingCachedAsync<T>(string name, T? defaultValue, CancellationToken token = default)
    {
        if (_cache.TryGetValue(name, out T? data))
        {
            return data;
        }

        data = await GetSettingAsync(name, defaultValue, token);

        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(name, data, options);

        return data;
    }
}