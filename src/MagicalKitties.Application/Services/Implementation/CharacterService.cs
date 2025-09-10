using System.Text.Json;
using FluentValidation;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Characters.Upgrades;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;

namespace MagicalKitties.Application.Services.Implementation;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IMagicalPowerRepository _magicalPowerRepository;
    private readonly ITalentRepository _talentRepository;
    private readonly IValidator<Character> _characterValidator;
    private readonly IValidator<GetAllCharactersOptions> _optionsValidator;

    public CharacterService(ICharacterRepository characterRepository, IValidator<Character> characterValidator, IValidator<GetAllCharactersOptions> optionsValidator, IMagicalPowerRepository magicalPowerRepository, ITalentRepository talentRepository)
    {
        _characterRepository = characterRepository;
        _characterValidator = characterValidator;
        _optionsValidator = optionsValidator;
        _magicalPowerRepository = magicalPowerRepository;
        _talentRepository = talentRepository;
    }

    public async Task<bool> CreateAsync(Character character, CancellationToken token = default)
    {
        await _characterValidator.ValidateAndThrowAsync(character, token);

        bool result = await _characterRepository.CreateAsync(character, token);

        return result;
    }

    public async Task<Character> CopyAsync(Guid id, CancellationToken token = default)
    {
        Character? existingCharacter = await _characterRepository.GetByIdAsync(id, false, token);

        Character copiedCharacter = existingCharacter!.CreateCopy();

        await _characterRepository.CopyAsync(copiedCharacter, token);

        return copiedCharacter;
    }

    public Task<bool> ExistsByIdAsync(Guid characterId, CancellationToken token = default)
    {
        return _characterRepository.ExistsByIdAsync(characterId, token);
    }

    public async Task<Character?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        Character? result = await _characterRepository.GetByIdAsync(id, cancellationToken: token);

        if (result is not null)
        {
            foreach (Upgrade upgrade in result.Upgrades)
            {
                switch (upgrade.Option)
                {
                    case UpgradeOption.talent:
                        if(upgrade.Choice is not null)
                        {
                            GainTalentUpgrade talentChoice = JsonSerializer.Deserialize<GainTalentUpgrade>(upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                            Talent foundTalent = await _talentRepository.GetByIdAsync(talentChoice.TalentId, token);

                            if (foundTalent is not null)
                            {
                                result.Talents.Add(foundTalent);
                            }
                        }
                        break;
                    case UpgradeOption.magicalPower:
                        if (upgrade.Choice is not null)
                        {
                            NewMagicalPowerUpgrade magicalPowerUpgrade = JsonSerializer.Deserialize<NewMagicalPowerUpgrade>(upgrade.Choice.ToString(), JsonSerializerOptions.Web);
                            MagicalPower foundMagicalPower = await _magicalPowerRepository.GetByIdAsync(magicalPowerUpgrade.MagicalPowerId, token);

                            if (foundMagicalPower is not null)
                            {
                                result.MagicalPowers.Add(foundMagicalPower);
                            }
                        }
                        break;
                    case UpgradeOption.bonusFeature:
                    case UpgradeOption.attribute3:
                    case UpgradeOption.attribute4:
                    case UpgradeOption.owieLimit:
                    case UpgradeOption.treatsValue:
                    default:
                        continue;
                }
            }
        }

        return result;
    }

    public async Task<IEnumerable<Character>> GetAllAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);

        return await _characterRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(GetAllCharactersOptions options, CancellationToken token = default)
    {
        return await _characterRepository.GetCountAsync(options, token);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        bool exists = await _characterRepository.ExistsByIdAsync(id, token);

        if (!exists)
        {
            return false; // not found
        }

        return await _characterRepository.DeleteAsync(id, token);
    }
}