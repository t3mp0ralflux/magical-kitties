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

        Upgrade? existingUpgrade = character.Upgrades.FirstOrDefault(x => x.Block == update.Upgrade.Block && x.Id == update.Upgrade.Id);

        List<MagicalPower> magicalPowers = (await GetValuesCachedAsync("MagicPowers", async () => await _magicalPowerRepository.GetAllAsync(new GetAllMagicalPowersOptions { Page = 1, PageSize = 99 }, token))).ToList();
        List<Talent> talents = (await GetValuesCachedAsync("Talents", async () => await _talentRepository.GetAllAsync(new GetAllTalentsOptions { Page = 1, PageSize = 99 }, token))).ToList();

        if (existingUpgrade is not null)
        {
            // update
            switch (update.UpgradeOption)
            {
                // attributes have to check level rules and see if they can update to that value or not.
                case UpgradeOption.attribute3:
                case UpgradeOption.attribute4:
                    ImproveAttributeUpgrade? improveAttributeUpdate;
                    ImproveAttributeUpgrade? existingAttributeImprovement = null;

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

                    if (existingUpgrade.Choice is not null)
                    {
                        existingAttributeImprovement = JsonSerializer.Deserialize<ImproveAttributeUpgrade>(existingUpgrade.Choice.ToString(), JsonSerializerOptions.Web);
                    }

                    existingAttributeImprovement ??= improveAttributeUpdate;

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

                    existingAttributeImprovement.AttributeOption = improveAttributeUpdate.AttributeOption;

                    existingUpgrade.Choice = existingAttributeImprovement;
                    break;
                case UpgradeOption.bonusFeature:
                    BonusFeatureUpgrade? bonusFeatureUpdate;
                    BonusFeatureUpgrade? existingFeature = null;

                    try
                    {
                        bonusFeatureUpdate = JsonSerializer.Deserialize<BonusFeatureUpgrade>(update.Upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                    }
                    catch (Exception _)
                    {
                        throw new BadHttpRequestException("Bonus feature upgrade payload was incorrect. Please verify and try again.");
                    }

                    if (bonusFeatureUpdate is null)
                    {
                        throw new BadHttpRequestException("Bonus feature upgrade payload was incorrect. Please verify and try again.");
                    }

                    if (existingUpgrade.Choice is not null)
                    {
                        existingFeature = JsonSerializer.Deserialize<BonusFeatureUpgrade>(existingUpgrade.Choice.ToString(), JsonSerializerOptions.Web);
                    }

                    existingFeature ??= bonusFeatureUpdate;

                    if (bonusFeatureUpdate.IsNested)
                    {
                        if (bonusFeatureUpdate.NestedMagicalPowerId != existingFeature.NestedMagicalPowerId)
                        {
                            throw new ValidationException([new ValidationFailure("MagicalPower", $"Tried to update Magical Power '{bonusFeatureUpdate.MagicalPowerId}' but it was not found.")]);
                        }

                        if (bonusFeatureUpdate.BonusFeatureId == existingFeature.NestedBonusFeatureId)
                        {
                            break; // they're the same, no need to update
                        }

                        MagicalPower? magicalPower = magicalPowers.FirstOrDefault(x => x.Id == existingFeature.NestedMagicalPowerId);
                        if (magicalPower is null)
                        {
                            _logger.LogError("Saved nested Magical Power {MagicalPowerId} on Character {CharacterId} was not found", existingFeature.NestedMagicalPowerId, character.Id);
                            break; // do nothing 'cause this should never happen.
                        }

                        if (!magicalPower.BonusFeatures.Exists(x => x.Id == bonusFeatureUpdate.NestedBonusFeatureId))
                        {
                            throw new ValidationException([new ValidationFailure("BonusFeature", $"Bonus Feature '{bonusFeatureUpdate.NestedBonusFeatureId}' does not exist on Magical Power '{magicalPower.Id}'")]);
                        }

                        existingFeature.NestedBonusFeatureId = bonusFeatureUpdate.NestedBonusFeatureId;
                    }
                    else
                    {
                        if (bonusFeatureUpdate.MagicalPowerId != existingFeature.MagicalPowerId)
                        {
                            throw new ValidationException([new ValidationFailure("MagicalPower", $"Tried to update Magical Power '{bonusFeatureUpdate.MagicalPowerId}' but it was not found.")]);
                        }

                        MagicalPower? magicalPower = magicalPowers.FirstOrDefault(x => x.Id == existingFeature.MagicalPowerId);
                        if (magicalPower is null)
                        {
                            _logger.LogError("Saved Magical Power {MagicalPowerId} on Character {CharacterId} was not found", existingFeature.MagicalPowerId, character.Id);
                            break; // do nothing 'cause this should never happen.
                        }

                        if (bonusFeatureUpdate.BonusFeatureId.HasValue && !magicalPower.BonusFeatures.Exists(x => x.Id == bonusFeatureUpdate.BonusFeatureId))
                        {
                            throw new ValidationException([new ValidationFailure("BonusFeature", $"Bonus Feature '{bonusFeatureUpdate.BonusFeatureId}' does not exist on Magical Power '{magicalPower.Id}'")]);
                        }

                        existingFeature.BonusFeatureId = bonusFeatureUpdate.BonusFeatureId;
                    }

                    existingUpgrade.Choice = existingFeature;

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
                        if (upgradePayload.MagicalPowerId == existingMagicalPower.MagicalPowerId || upgradePayload.NestedMagicalPowerId == existingMagicalPower.MagicalPowerId)
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
        }
        else
        {
            // adding new one
            List<UpgradeRule> upgradeRules = await GetValuesCachedAsync("Rules", () => _upgradeRepository.GetRulesAsync(token));
            List<UpgradeRule> levelRules = upgradeRules.Where(x => x.Block <= update.Upgrade.Block).ToList();

            if (levelRules.FirstOrDefault(x => x.Id == update.Upgrade.Id) is null)
            {
                // out of range, throw hands
                throw new ValidationException([new ValidationFailure("InvalidOption", "Option selected was outside the available options for this character.")]);
            }

            bool foundReference = false;
            switch (update.UpgradeOption)
            {
                case UpgradeOption.bonusFeature:
                    if (update.Upgrade.Choice is null)
                    {
                        break;
                    }

                    BonusFeatureUpgrade bonusFeatureUpdateChoice;

                    try
                    {
                        bonusFeatureUpdateChoice = JsonSerializer.Deserialize<BonusFeatureUpgrade>(update.Upgrade.Choice!.ToString()!, JsonSerializerOptions.Web)!;
                    }
                    catch (Exception _)
                    {
                        throw new BadHttpRequestException("Bonus feature upgrade payload was formatted incorrectly. Please verify and try again.");
                    }

                    MagicalPower? foundMagicalPower = magicalPowers.FirstOrDefault(x => x.Id == bonusFeatureUpdateChoice!.MagicalPowerId);

                    if (foundMagicalPower is null)
                    {
                        throw new ValidationException([new ValidationFailure("MagicalPower", $"Magical Power '{bonusFeatureUpdateChoice!.MagicalPowerId}' is invalid.")]);
                    }

                    if (foundMagicalPower.BonusFeatures.FirstOrDefault(x => x.Id == bonusFeatureUpdateChoice!.BonusFeatureId.GetValueOrDefault()) is null)
                    {
                        throw new ValidationException([new ValidationFailure("BonusFeature", $"Bonus Feature '{bonusFeatureUpdateChoice!.BonusFeatureId}' is invalid for Magical Power '{bonusFeatureUpdateChoice.MagicalPowerId}'.")]);
                    }

                    List<Upgrade> existingBonusFeatureUpgrades = character.Upgrades.Where(x => x.Option == UpgradeOption.bonusFeature).ToList();

                    if (existingBonusFeatureUpgrades
                        .Select(existingBonusFeatureUpgrade => JsonSerializer.Deserialize<BonusFeatureUpgrade>(existingBonusFeatureUpgrade.Choice!.ToString()!, JsonSerializerOptions.Web))
                        .Any(existingChoice => existingChoice!.BonusFeatureId == bonusFeatureUpdateChoice!.BonusFeatureId && existingChoice.MagicalPowerId == bonusFeatureUpdateChoice.MagicalPowerId))
                    {
                        throw new ValidationException([new ValidationFailure("BonusFeature", $"Bonus Feature '{bonusFeatureUpdateChoice!.BonusFeatureId}' for Magical Power '{bonusFeatureUpdateChoice.MagicalPowerId}' was already present on Character.")]);
                    }

                    MagicalPower? primaryMagicalPower = character.MagicalPowers.FirstOrDefault();

                    if (primaryMagicalPower is not null)
                    {
                        if (primaryMagicalPower.Id == bonusFeatureUpdateChoice!.MagicalPowerId)
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

                    break;
                case UpgradeOption.talent:
                    // check for existing Talent and throw hands if found
                    break;
                case UpgradeOption.magicalPower:
                    // check for existing MP and throw hands if found
                    break;
                case UpgradeOption.treatsValue:
                case UpgradeOption.attribute3:
                case UpgradeOption.attribute4:
                case UpgradeOption.owieLimit:
                default:
                    break;
            }

            existingUpgrade = update.Upgrade;

            character.Upgrades.Add(existingUpgrade);
        }

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
                if (upgradeInfo!.MagicalPowerId == choice!.MagicalPowerId || (upgradeInfo.NestedMagicalPowerId.HasValue && upgradeInfo.NestedMagicalPowerId.Value == choice.MagicalPowerId))
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