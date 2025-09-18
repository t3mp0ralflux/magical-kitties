using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Characters.Upgrades;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterUpgradeService : ICharacterUpgradeService
{
    private readonly IMemoryCache _cache;
    private readonly ICharacterRepository _characterRepository;
    private readonly ILogger<CharacterUpgradeService> _logger;
    private readonly IMagicalPowerRepository _magicalPowerRepository;
    private readonly ITalentRepository _talentRepository;
    private readonly IUpgradeRepository _upgradeRepository;

    public CharacterUpgradeService(ICharacterRepository characterRepository, IUpgradeRepository upgradeRepository, IMagicalPowerRepository magicalPowerRepository, ITalentRepository talentRepository, IMemoryCache cache, ILogger<CharacterUpgradeService> logger)
    {
        _characterRepository = characterRepository;
        _upgradeRepository = upgradeRepository;
        _magicalPowerRepository = magicalPowerRepository;
        _talentRepository = talentRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> UpsertUpgradeAsync(UpgradeRequest update, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(update.CharacterId, cancellationToken: token);

        if (character is null)
        {
            return false;
        }

        int characterBlock = GetLevelBlock(character.Level);

        if (update.Upgrade.Block > characterBlock)
        {
            throw new ValidationException([new ValidationFailure("Upgrade", "Cannot add upgrade. Upgrade is higher than the character's level.")]);
        }
        
        List<UpgradeRule> upgradeRules = await GetValuesCachedAsync("Rules", () => _upgradeRepository.GetRulesAsync(token));
        List<UpgradeRule> levelRules = upgradeRules.Where(x => x.Block <= update.Upgrade.Block).ToList();

        if (levelRules.FirstOrDefault(x => x.Id == update.Upgrade.Id) is null)
        {
            // out of range, throw hands
            throw new ValidationException([new ValidationFailure("InvalidOption", "Option selected was outside the available options for this character.")]);
        }

        Upgrade? existingUpgrade = character.Upgrades.FirstOrDefault(x => x.Block == update.Upgrade.Block && x.Id == update.Upgrade.Id);

        List<MagicalPower> magicalPowers = (await GetValuesCachedAsync("MagicPowers", async () => await _magicalPowerRepository.GetAllAsync(new GetAllMagicalPowersOptions { Page = 1, PageSize = 99 }, token))).ToList();
        List<Talent> talents = (await GetValuesCachedAsync("Talents", async () => await _talentRepository.GetAllAsync(new GetAllTalentsOptions { Page = 1, PageSize = 99 }, token))).ToList();

        bool foundReference = false;
        switch (update.UpgradeOption)
        {
            // attributes have to check level rules and see if they can update to that value or not.
            case UpgradeOption.attribute3:
            case UpgradeOption.attribute4:
                ImproveAttributeUpgrade? improveAttributeUpdate;
                
                try
                {
                    improveAttributeUpdate = JsonSerializer.Deserialize<ImproveAttributeUpgrade>(update.Upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                }
                catch (Exception ex)
                {
                    throw new BadHttpRequestException("Improve Attribute upgrade payload was incorrect. Please verify and try again.");
                }

                if (improveAttributeUpdate is null)
                {
                    throw new BadHttpRequestException("Improve Attribute upgrade payload was incorrect. Please verify and try again.");
                }
                
                int maxPropertyValue = improveAttributeUpdate.AttributeOption switch
                {
                    AttributeOption.cunning => character.Cunning,
                    AttributeOption.cute => character.Cute,
                    AttributeOption.fierce => character.Fierce,
                    _ => throw new ValidationException([new ValidationFailure("UpgradeOption", "Attribute upgrade option was not valid.")])
                };

                if (update.UpgradeOption == UpgradeOption.attribute3)
                {
                    if (character.Level < 5)
                    {
                        if (maxPropertyValue + 1 > 3)
                        {
                            throw new ValidationException([new ValidationFailure("AttributeMax3", $"Level {character.Level} characters cannot have any Attribute above 3.")]);
                        }
                    }
                }
                else
                {
                    if (character.Level < 5)
                    {
                        throw new ValidationException([new ValidationFailure("CharacterLevel", "This upgrade is invalid for characters less than level 5.")]);
                    }

                    if (maxPropertyValue + 1 > 4)
                    {
                        throw new ValidationException([new ValidationFailure("AttributeMaxLevel", $"Level {character.Level} characters cannot have an Attribute above 3.")]);
                    }
                }
                break;
            case UpgradeOption.bonusFeature:
                // can't verify what's not there
                if (update.Upgrade.Choice is not null)
                {
                    BonusFeatureUpgrade bonusFeatureUpdateChoice;
                    try
                    {
                        bonusFeatureUpdateChoice = JsonSerializer.Deserialize<BonusFeatureUpgrade>(update.Upgrade.Choice.ToString(), JsonSerializerOptions.Web)!;
                    }
                    catch (Exception _)
                    {
                        throw new BadHttpRequestException("Bonus feature upgrade payload was formatted incorrectly. Please verify and try again.");
                    }

                    MagicalPower? foundMagicalPower = magicalPowers.FirstOrDefault(x => x.Id == bonusFeatureUpdateChoice.MagicalPowerId);

                    if (foundMagicalPower is null)
                    {
                        throw new ValidationException([new ValidationFailure("MagicalPower", $"Magical Power '{bonusFeatureUpdateChoice.MagicalPowerId}' is invalid.")]);
                    }

                    if (bonusFeatureUpdateChoice.BonusFeatureId.HasValue)
                    {
                        if (foundMagicalPower.BonusFeatures.FirstOrDefault(x => x.Id == bonusFeatureUpdateChoice.BonusFeatureId.GetValueOrDefault()) is null)
                        {
                            throw new ValidationException([new ValidationFailure("BonusFeature", $"Bonus Feature '{bonusFeatureUpdateChoice.BonusFeatureId}' is invalid for Magical Power '{bonusFeatureUpdateChoice.MagicalPowerId}'.")]);
                        }

                        List<Upgrade> existingBonusFeatureUpgrades = character.Upgrades.Where(x => x.Option == UpgradeOption.bonusFeature).ToList();

                        if (existingBonusFeatureUpgrades
                            .Select(existingBonusFeatureUpgrade => JsonSerializer.Deserialize<BonusFeatureUpgrade>(existingBonusFeatureUpgrade.Choice!.ToString()!, JsonSerializerOptions.Web))
                            .Any(existingChoice => existingChoice!.BonusFeatureId == bonusFeatureUpdateChoice.BonusFeatureId && existingChoice.MagicalPowerId == bonusFeatureUpdateChoice.MagicalPowerId))
                        {
                            throw new ValidationException([new ValidationFailure("BonusFeature", $"Bonus Feature '{bonusFeatureUpdateChoice!.BonusFeatureId}' for Magical Power '{bonusFeatureUpdateChoice.MagicalPowerId}' was already present on Character.")]);
                        }
                    }

                    MagicalPower? primaryMagicalPower = character.MagicalPowers.FirstOrDefault();

                    if (primaryMagicalPower is not null)
                    {
                        if (primaryMagicalPower.Id == bonusFeatureUpdateChoice.MagicalPowerId)
                        {
                            foundReference = true;
                        }
                    }

                    Upgrade? foundMagicalPowerUpgrade = character.Upgrades.FirstOrDefault(x => x.Option == UpgradeOption.magicalPower);

                    if (foundMagicalPowerUpgrade?.Choice is not null)
                    {
                        NewMagicalPowerUpgrade? magicalPowerChoice = JsonSerializer.Deserialize<NewMagicalPowerUpgrade>(foundMagicalPowerUpgrade.Choice.ToString()!, JsonSerializerOptions.Web);

                        if (magicalPowerChoice!.MagicalPowerId == bonusFeatureUpdateChoice.MagicalPowerId)
                        {
                            foundReference = true;
                        }
                    }

                    if (!foundReference)
                    {
                        throw new ValidationException([new ValidationFailure("MagicalPower", $"Magical Power '{bonusFeatureUpdateChoice.MagicalPowerId}' was not present on Character.")]);
                    }
                }

                if (existingUpgrade is not null)
                {
                    existingUpgrade.Choice = JsonSerializer.Deserialize<BonusFeatureUpgrade>(update.Upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                }
                else
                {
                    existingUpgrade = new Upgrade
                                      {
                                          Id = update.Upgrade.Id,
                                          Block = update.Upgrade.Block,
                                          Option = update.Upgrade.Option,
                                      };
                    
                    if (update.Upgrade.Choice is not null)
                    {
                        existingUpgrade.Choice = JsonSerializer.Deserialize<BonusFeatureUpgrade>(update.Upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                    }
                    
                    character.Upgrades.Add(existingUpgrade);
                }
                
                break;
            case UpgradeOption.talent:
                GainTalentUpgrade? talentUpdate;
                GainTalentUpgrade? existingTalent = null;

                try
                {
                    talentUpdate = JsonSerializer.Deserialize<GainTalentUpgrade>(update.Upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                }
                catch (Exception _)
                {
                    throw new BadHttpRequestException("Talent upgrade payload was incorrect. Please verify and try again.");
                }

                if (talentUpdate is null)
                {
                    throw new BadHttpRequestException("Talent upgrade payload was incorrect. Please verify and try again.");
                }

                if (existingUpgrade.Choice is not null)
                {
                    existingTalent = JsonSerializer.Deserialize<GainTalentUpgrade>(existingUpgrade.Choice.ToString(), JsonSerializerOptions.Web);
                }

                existingTalent ??= talentUpdate;

                if (talents.FirstOrDefault(x => x.Id == existingTalent.TalentId) is null)
                {
                    _logger.LogError("Saved Talent {TalentId} on Character {CharacterId} was not found", existingTalent.TalentId, character.Id);
                    break; // do nothing 'cause this should never happen.
                }

                if (talents.FirstOrDefault(x => x.Id == talentUpdate.TalentId) is null)
                {
                    throw new ValidationException([new ValidationFailure("Talent", $"Talent '{talentUpdate.TalentId}' does not exist.")]);
                }

                if (character.Talents.FirstOrDefault(x => x.Id == talentUpdate.TalentId) is not null)
                {
                    throw new ValidationException([new ValidationFailure("TalentExists", "Talent already present on character.")]);
                }

                existingTalent.TalentId = talentUpdate.TalentId;

                existingUpgrade.Choice = existingTalent;
                break;
            case UpgradeOption.magicalPower:
                NewMagicalPowerUpgrade? magicalPowerUpdate;
                NewMagicalPowerUpgrade? existingMagicalPower = null;

                try
                {
                    magicalPowerUpdate = JsonSerializer.Deserialize<NewMagicalPowerUpgrade>(update.Upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                }
                catch
                {
                    throw new BadHttpRequestException("MagicalPower update payload was incorrect. Please verify and try again.");
                }

                if (magicalPowerUpdate is null)
                {
                    throw new BadHttpRequestException("MagicalPower update payload was incorrect. Please verify and try again.");
                }

                if (existingUpgrade.Choice is not null)
                {
                    existingMagicalPower = JsonSerializer.Deserialize<NewMagicalPowerUpgrade>(existingUpgrade.Choice.ToString(), JsonSerializerOptions.Web);
                }

                existingMagicalPower ??= magicalPowerUpdate;

                if (magicalPowers.FirstOrDefault(x => x.Id == existingMagicalPower.MagicalPowerId) is null)
                {
                    _logger.LogError("Saved Magical Power {MagicalPowerId} on Character {CharacterId} was not found", existingMagicalPower.MagicalPowerId, character.Id);
                    break; // do nothing 'cause this shouldn't happen
                }

                if (magicalPowers.FirstOrDefault(x => x.Id == magicalPowerUpdate.MagicalPowerId) is null)
                {
                    throw new ValidationException([new ValidationFailure("MagicalPower", $"Magical Power '{magicalPowerUpdate.MagicalPowerId}' does not exist.")]);
                }

                if (character.MagicalPowers.FirstOrDefault(x => x.Id == magicalPowerUpdate.MagicalPowerId) is not null
                    || character.MagicalPowers.FirstOrDefault(x => x.BonusFeatures.Any(y => y.Id == magicalPowerUpdate.MagicalPowerId)) is not null)
                {
                    throw new ValidationException([new ValidationFailure("MagicalPowerExists", "Magical Power already present on character.")]);
                }

                List<Upgrade> affectedUpgrades = character.Upgrades.Where(x => x.Option == UpgradeOption.bonusFeature).ToList();
                foreach (Upgrade affectedUpgrade in affectedUpgrades)
                {
                    if (affectedUpgrade.Choice is null)
                    {
                        continue;
                    }

                    BonusFeatureUpgrade upgradePayload = JsonSerializer.Deserialize<BonusFeatureUpgrade>(affectedUpgrade.Choice.ToString(), JsonSerializerOptions.Web);
                    if (upgradePayload.MagicalPowerId == existingMagicalPower.MagicalPowerId)
                    {
                        // clear out the existing bonus feature information cause it'll be wrong.
                        affectedUpgrade.Choice = null;
                    }
                }

                existingMagicalPower.MagicalPowerId = magicalPowerUpdate.MagicalPowerId;

                existingUpgrade.Choice = existingMagicalPower;
                break;
            case UpgradeOption.owieLimit:
            case UpgradeOption.treatsValue:
                break; // doesn't really do anything, honestly
            default:
                throw new ValidationException([new ValidationFailure("UpgradeOption", "Upgrade option was not valid.")]);
        }
        
        // if (existingUpgrade is null)
        // {
        //     character.Upgrades.Add(update.Upgrade);
        // }

        return await _upgradeRepository.UpsertUpgradesAsync(character.Id, character.Upgrades, token);
    }

    public async Task<bool> RemoveUpgradeAsync(UpgradeRequest update, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(update.CharacterId, cancellationToken: token);

        Upgrade? upgrade = character?.Upgrades.FirstOrDefault(x => x.Id == update.Upgrade.Id);

        if (upgrade is null)
        {
            return false;
        }

        bool featureFound = false;

        if (update.UpgradeOption == UpgradeOption.magicalPower && upgrade.Choice is not null)
        {
            NewMagicalPowerUpgrade choice = JsonSerializer.Deserialize<NewMagicalPowerUpgrade>(upgrade.Choice!.ToString()!, JsonSerializerOptions.Web)!;
            List<Upgrade> affectedUpgrades = character?.Upgrades.Where(x => x.Option == UpgradeOption.bonusFeature).ToList() ?? [];

            foreach (Upgrade affectedUpgrade in affectedUpgrades)
            {
                if (affectedUpgrade.Choice is null)
                {
                    continue;
                }

                BonusFeatureUpgrade upgradeInfo = JsonSerializer.Deserialize<BonusFeatureUpgrade>(affectedUpgrade.Choice!.ToString()!, JsonSerializerOptions.Web)!;
                if (upgradeInfo.MagicalPowerId == choice.MagicalPowerId)
                {
                    featureFound = true;
                }
            }
        }

        if (featureFound)
        {
            throw new ValidationException([new ValidationFailure("MagicalPowerExists", "MagicalPower still exists on Character. Cannot remove upgrade.")]); // mostly for FE debugging and BE shenanigans
        }

        character!.Upgrades.Remove(upgrade);

        return await _upgradeRepository.UpsertUpgradesAsync(character.Id, character.Upgrades, token);
    }

    private static int GetLevelBlock(int level)
    {
        return (int)Math.Round(level * 0.3 + .02);
    }

    private async Task<T> GetValuesCachedAsync<T>(string name, Func<Task<T>> function)
    {
        if (_cache.TryGetValue(name, out T? data))
        {
            return data!;
        }

        data = await Task.Run(function.Invoke);

        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));

        _cache.Set(name, data, options);

        return data;
    }
}