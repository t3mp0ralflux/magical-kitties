using FluentValidation;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Characters.Upgrades;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterUpgradeService : ICharacterUpgradeService
{
    private readonly IMemoryCache _cache;
    private readonly ICharacterRepository _characterRepository;
    private readonly IMagicalPowerRepository _magicalPowerRepository;
    private readonly ITalentRepository _talentRepository;
    private readonly IUpgradeRepository _upgradeRepository;

    public CharacterUpgradeService(ICharacterRepository characterRepository, IUpgradeRepository upgradeRepository, IMagicalPowerRepository magicalPowerRepository, ITalentRepository talentRepository, IMemoryCache cache)
    {
        _characterRepository = characterRepository;
        _upgradeRepository = upgradeRepository;
        _magicalPowerRepository = magicalPowerRepository;
        _talentRepository = talentRepository;
        _cache = cache;
    }

    public async Task<bool> UpsertUpgradeAsync(UpgradeRequest update, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(update.AccountId, update.CharacterId, cancellationToken: token);

        if (character is null)
        {
            return false;
        }

        int characterBlock = GetLevelBlock(character.Level);

        if (update.Upgrade.Block > characterBlock)
        {
            throw new ValidationException("Cannot add upgrade. Upgrade is higher than the character's level.");
        }

        Upgrade? existingUpgrade = character.Upgrades.FirstOrDefault(x => x.Block == update.Upgrade.Block && x.Id == update.Upgrade.Id);

        // update
        if (existingUpgrade is not null)
        {
            List<MagicalPower> magicalPowers = (await GetValuesCachedAsync("MagicPowers", async () => await _magicalPowerRepository.GetAllAsync(new GetAllMagicalPowersOptions { Page = 1, PageSize = 99 }, token))).ToList();
            List<Talent> talents = (await GetValuesCachedAsync("Talents", async () => await _talentRepository.GetAllAsync(new GetAllTalentsOptions { Page = 1, PageSize = 99 }, token))).ToList();

            switch (update.UpgradeOption)
            {
                // attributes have to check level rules and see if they can update to that value or not.
                case UpgradeOption.attribute3:
                case UpgradeOption.attribute4:
                    int maxPropertyValue = update.Upgrade.Option switch
                    {
                        AttributeOption.cunning => character.Cunning,
                        AttributeOption.cute => character.Cute,
                        AttributeOption.fierce => character.Fierce,
                        _ => throw new ValidationException("Attribute upgrade option was not valid.")
                    };

                    if (update.UpgradeOption == UpgradeOption.attribute3)
                    {
                        if (character.Level < 5)
                        {
                            if (maxPropertyValue + 1 > 3)
                            {
                                throw new ValidationException($"Level {character.Level} characters cannot have any Attribute above 3.");
                            }
                        }
                    }
                    else
                    {
                        if (character.Level < 5)
                        {
                            throw new ValidationException("This upgrade is invalid for characters less than level 5.");
                        }

                        if (maxPropertyValue + 1 > 4)
                        {
                            throw new ValidationException($"Level {character.Level} characters cannot have an Attribute above 3.");
                        }
                    }

                    if (existingUpgrade.Option == update.Upgrade.Option)
                    {
                        break; // they're the same, no bother to do anything.
                    }

                    existingUpgrade.Option = update.Upgrade.Option;
                    break;

                case UpgradeOption.bonusFeature:
                    BonusFeatureUpgrade bonusFeatureUpdate = (BonusFeatureUpgrade)update.Upgrade.Choice!;
                    BonusFeatureUpgrade existingFeature = (BonusFeatureUpgrade)existingUpgrade.Choice!;

                    if (bonusFeatureUpdate.IsNested)
                    {
                        if (bonusFeatureUpdate.NestedMagicalPowerId != existingFeature.NestedMagicalPowerId)
                        {
                            throw new ValidationException($"Tried to update Magical Power '{bonusFeatureUpdate.MagicalPowerId}' but it was not found.");
                        }

                        if (bonusFeatureUpdate.BonusFeatureId == existingFeature.NestedBonusFeatureId)
                        {
                            break; // they're the same, no need to update
                        }

                        MagicalPower? magicalPower = magicalPowers.FirstOrDefault(x => x.Id == existingFeature.NestedMagicalPowerId);
                        if (magicalPower is null)
                        {
                            Log.Logger.Error("Saved nested Magical Power {MagicalPowerId} on Character {CharacterId} was not found", existingFeature.NestedMagicalPowerId, character.Id);
                            break; // do nothing 'cause this should never happen.
                        }

                        if (!magicalPower.BonusFeatures.Exists(x => x.Id == bonusFeatureUpdate.NestedBonusFeatureId))
                        {
                            throw new ValidationException($"Bonus Feature '{bonusFeatureUpdate.NestedBonusFeatureId}' does not exist on Magical Power '{magicalPower.Id}'");
                        }

                        existingFeature.NestedBonusFeatureId = bonusFeatureUpdate.NestedBonusFeatureId;
                    }
                    else
                    {
                        if (bonusFeatureUpdate.MagicalPowerId != existingFeature.MagicalPowerId)
                        {
                            throw new ValidationException($"Tried to update Magical Power '{bonusFeatureUpdate.MagicalPowerId}' but it was not found.");
                        }

                        if (bonusFeatureUpdate.BonusFeatureId == existingFeature.BonusFeatureId)
                        {
                            break; // they're the same, no need to update
                        }

                        MagicalPower? magicalPower = magicalPowers.FirstOrDefault(x => x.Id == existingFeature.MagicalPowerId);
                        if (magicalPower is null)
                        {
                            Log.Logger.Error("Saved Magical Power {MagicalPowerId} on Character {CharacterId} was not found", existingFeature.MagicalPowerId, character.Id);
                            break; // do nothing 'cause this should never happen.
                        }

                        if (!magicalPower.BonusFeatures.Exists(x => x.Id == bonusFeatureUpdate.BonusFeatureId))
                        {
                            throw new ValidationException($"Bonus Feature '{bonusFeatureUpdate.BonusFeatureId}' does not exist on Magical Power '{magicalPower.Id}'");
                        }

                        existingFeature.BonusFeatureId = bonusFeatureUpdate.BonusFeatureId;
                    }

                    existingUpgrade.Choice = existingFeature;
                    break;
                case UpgradeOption.talent:
                    GainTalentUpgrade talentUpdate = (GainTalentUpgrade)update.Upgrade.Choice!;
                    GainTalentUpgrade existingTalent = (GainTalentUpgrade)existingUpgrade.Choice!;

                    if (talentUpdate.TalentId == existingTalent.TalentId)
                    {
                        break; // nothing to see here, it's the same
                    }

                    if (talents.FirstOrDefault(x => x.Id == existingTalent.TalentId) is null)
                    {
                        Log.Logger.Error("Saved Talent {TalentId} on Character {CharacterId} was not found", existingTalent.TalentId, character.Id);
                        break; // do nothing 'cause this should never happen.
                    }

                    if (talents.FirstOrDefault(x => x.Id == talentUpdate.TalentId) is null)
                    {
                        throw new ValidationException($"Talent '{talentUpdate.TalentId}' does not exist.");
                    }

                    if (character.Talents.FirstOrDefault(x => x.Id == talentUpdate.TalentId) is not null)
                    {
                        throw new ValidationException("Talent already present on character.");
                    }

                    existingTalent.TalentId = talentUpdate.TalentId;

                    existingUpgrade.Choice = existingTalent;
                    break;
                case UpgradeOption.magicalPower:
                    NewMagicalPowerUpgrade magicalPowerUpdate = (NewMagicalPowerUpgrade)update.Upgrade.Choice!;
                    NewMagicalPowerUpgrade existingMagicalPower = (NewMagicalPowerUpgrade)existingUpgrade.Choice!;

                    if (magicalPowerUpdate.MagicalPowerId == existingMagicalPower.MagicalPowerId)
                    {
                        break; // not changing the magical power, skip.
                    }

                    if (magicalPowers.FirstOrDefault(x => x.Id == existingMagicalPower.MagicalPowerId) is null)
                    {
                        Log.Logger.Error("Saved Magical Power {MagicalPowerId} on Character {CharacterId} was not found", existingMagicalPower.MagicalPowerId, character.Id);
                        break; // do nothing 'cause this shouldn't happen
                    }

                    if (magicalPowers.FirstOrDefault(x => x.Id == magicalPowerUpdate.MagicalPowerId) is null)
                    {
                        throw new ValidationException($"Magical Power '{magicalPowerUpdate.MagicalPowerId}' does not exist.");
                    }

                    if (character.MagicalPowers.FirstOrDefault(x => x.Id == magicalPowerUpdate.MagicalPowerId) is not null
                        || character.MagicalPowers.FirstOrDefault(x => x.BonusFeatures.Any(y => y.Id == magicalPowerUpdate.MagicalPowerId)) is not null)
                    {
                        throw new ValidationException("Magical Power already present on character.");
                    }

                    existingMagicalPower.MagicalPowerId = magicalPowerUpdate.MagicalPowerId;

                    existingUpgrade.Choice = existingMagicalPower;
                    break;
                case UpgradeOption.owieLimit:
                case UpgradeOption.treatsValue:
                    break; // doesn't really do anything, honestly
                default:
                    throw new ValidationException("Upgrade option was not valid.");
            }

            await _upgradeRepository.UpsertUpgradesAsync(character.Id, character.Upgrades, token);

            return true;
        }

        // adding new one
        List<UpgradeRule> upgradeRules = await GetValuesCachedAsync("Rules", () => _upgradeRepository.GetRulesAsync(token));
        List<UpgradeRule> levelRules = upgradeRules.Where(x => x.Block <= update.Upgrade.Block).ToList();

        if (levelRules.FirstOrDefault(x => x.UpgradeChoice == update.Upgrade.Id) is null)
        {
            // out of range, throw hands
            throw new ValidationException("Option selected was outside the available options for this character.");
        }

        existingUpgrade = update.Upgrade;

        character.Upgrades.Add(existingUpgrade);

        return await _upgradeRepository.UpsertUpgradesAsync(character.Id, character.Upgrades, token);
    }

    public async Task<bool> RemoveUpgradeAsync(UpgradeRequest update, CancellationToken token = default)
    {
        Character? character = await _characterRepository.GetByIdAsync(update.AccountId, update.CharacterId, cancellationToken: token);

        Upgrade? upgrade = character?.Upgrades.FirstOrDefault(x => x.Id == update.Upgrade.Id);

        if (upgrade is null)
        {
            return false;
        }

        switch (update.UpgradeOption)
        {
            case UpgradeOption.talent:
                GainTalentUpgrade talentUpgrade = (GainTalentUpgrade)upgrade.Choice!;

                if (character!.Talents.FirstOrDefault(x => x.Id == talentUpgrade.TalentId) is not null)
                {
                    throw new ValidationException("Talent still exists on Character. Cannot remove upgrade."); // mostly for FE debugging and BE shenanigans
                }

                break;
            case UpgradeOption.magicalPower:
                NewMagicalPowerUpgrade magicalPowerUpgrade = (NewMagicalPowerUpgrade)upgrade.Choice!;

                if (character!.MagicalPowers.FirstOrDefault(x => x.Id == magicalPowerUpgrade.MagicalPowerId) is not null)
                {
                    throw new ValidationException("MagicalPower still exists on Character. Cannot remove upgrade."); // mostly for FE debugging and BE shenanigans
                }

                break;
            case UpgradeOption.bonusFeature:
            case UpgradeOption.attribute3:
            case UpgradeOption.attribute4:
            case UpgradeOption.owieLimit:
            case UpgradeOption.treatsValue:
            default:
                break; // no validation needed on these
        }

        character!.Upgrades.Remove(upgrade);

        return await _upgradeRepository.UpsertUpgradesAsync(character.Id, character.Upgrades, token);
    }

    private int GetLevelBlock(int level)
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