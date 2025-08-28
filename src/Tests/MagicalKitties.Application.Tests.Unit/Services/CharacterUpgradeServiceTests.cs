using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Specialized;
using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Characters.Upgrades;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services.Implementation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class CharacterUpgradeServiceTests
{
    private readonly ICharacterRepository _characterRepository = Substitute.For<ICharacterRepository>();
    private readonly IMagicalPowerRepository _magicalPowerRepository = Substitute.For<IMagicalPowerRepository>();
    private readonly IMemoryCache _memoryCache = Substitute.For<IMemoryCache>();
    public readonly CharacterUpgradeService _sut;
    private readonly ITalentRepository _talentRepository = Substitute.For<ITalentRepository>();
    private readonly IUpgradeRepository _upgradeRepository = Substitute.For<IUpgradeRepository>();
    private readonly ILogger<CharacterUpgradeService> _logger = Substitute.For<ILogger<CharacterUpgradeService>>();

    public CharacterUpgradeServiceTests()
    {
        _sut = new CharacterUpgradeService(_characterRepository, _upgradeRepository, _magicalPowerRepository, _talentRepository, _memoryCache, _logger);
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldReturnFalse_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = Guid.NewGuid(),
                                    UpgradeOption = UpgradeOption.attribute3,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = Guid.NewGuid(),
                                                  Block = 1,
                                                  Option = UpgradeOption.attribute3
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns((Character?)null);

        // Act
        bool result = await _sut.UpsertUpgradeAsync(update);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenUpgradeIsAboveCharacterBlock()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        character.Level = 2;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.talent,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = Guid.NewGuid(),
                                                  Block = 2,
                                                  Option = UpgradeOption.talent,
                                                  Choice = "42"
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("Upgrade");
        error.ErrorMessage.Should().Be("Cannot add upgrade. Upgrade is higher than the character's level.");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenUpgradeChoiceDoesNotExistAndNewUpgradeIsAdded()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        character.Level = 2;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.talent,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = Guid.NewGuid(),
                                                  Block = 1,
                                                  Option = UpgradeOption.bonusFeature
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(Fakes.GenerateUpgradeRules());

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("InvalidOption");
        error.ErrorMessage.Should().Be("Option selected was outside the available options for this character.");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldAddUpgrade_WhenUpgradeIsAdded()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        
        character.Level = 5;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.talent,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x.UpgradeOption == UpgradeOption.talent).Id,
                                                  Block = 2,
                                                  Option = UpgradeOption.talent
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(rules);
        _upgradeRepository.UpsertUpgradesAsync(character.Id, Arg.Any<List<Upgrade>>()).Returns(true);

        // Act
        bool result = await _sut.UpsertUpgradeAsync(update);

        // Assert
        result.Should().BeTrue();
    }

    #region Attribute (Max3) Upgrade

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenAttribute3UpgradeExistsAndOptionIsInvalid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithUpgrades(rules);
        character.Level = 2;

        Enum.TryParse(typeof(AttributeOption), "-1", out object? o);

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.attribute3,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x is { UpgradeOption: UpgradeOption.attribute3, Block: 1 }).Id,
                                                  Block = 1,
                                                  Option = UpgradeOption.attribute3,
                                                  Choice = JsonSerializer.Serialize(new ImproveAttributeUpgrade
                                                           {
                                                               AttributeOption = (AttributeOption)o
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(Fakes.GenerateUpgradeRules());
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers());
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("UpgradeOption");
        error.ErrorMessage.Should().Be("Attribute upgrade option was not valid.");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenAttribute3UpgradeExistsAndOptionInvalidatesRules()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);
        character.Level = 2;

        // generator has Cunning set to 3. Below level 5 can't have any attribute above 3.
        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.attribute3,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x is { UpgradeOption: UpgradeOption.attribute3, Block: 1 }).Id,
                                                  Block = 1,
                                                  Option = UpgradeOption.attribute3,
                                                  Choice = JsonSerializer.Serialize(new ImproveAttributeUpgrade
                                                           {
                                                               AttributeOption = AttributeOption.cunning
                                                           }, JsonSerializerOptions.Web)
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(Fakes.GenerateUpgradeRules());
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers());
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("AttributeMax3");
        error.ErrorMessage.Should().Be("Level 2 characters cannot have any Attribute above 3.");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldUpdateAttribute_WhenAttribute3UpgradeExistsAndOptionIsValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);
        character.Level = 2;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.attribute3,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x is { UpgradeOption: UpgradeOption.attribute3, Block: 1 }).Id,
                                                  Block = 1,
                                                  Option = UpgradeOption.attribute3,
                                                  Choice = JsonSerializer.Serialize(new ImproveAttributeUpgrade
                                                           {
                                                               AttributeOption = AttributeOption.cute
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(Fakes.GenerateUpgradeRules());
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers());
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());

        await _upgradeRepository.UpsertUpgradesAsync(character.Id, Arg.Do<List<Upgrade>>(arg => character.Upgrades = arg));

        // Act
        bool result = await _sut.UpsertUpgradeAsync(update);

        // Assert
        result.Should().BeTrue();
        character.Upgrades.FirstOrDefault(x => x.Option == UpgradeOption.attribute3).Should().NotBeNull();
    }

    #endregion

    #region Bonus Feature

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenBonusFeatureUpgradeExistsAndMagicalPowerNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);
        character.Level = 2;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.bonusFeature,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x is { UpgradeOption: UpgradeOption.bonusFeature, Block: 1 }).Id,
                                                  Block = 1,
                                                  Choice = JsonSerializer.Serialize(new BonusFeatureUpgrade
                                                           {
                                                               MagicalPowerId = 22,
                                                               BonusFeatureId = 1,
                                                               IsNested = false
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(Fakes.GenerateUpgradeRules());
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers());
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("MagicalPower");
        error.ErrorMessage.Should().Be("Tried to update Magical Power '22' but it was not found.");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenBonusFeatureUpgradeExistsAndMagicalPowerBonusDoesNotExist()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);
        character.Level = 2;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.bonusFeature,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x is { UpgradeOption: UpgradeOption.bonusFeature, Block: 1 }).Id,
                                                  Block = 1,
                                                  Choice = JsonSerializer.Serialize(new BonusFeatureUpgrade
                                                           {
                                                               MagicalPowerId = 33,
                                                               BonusFeatureId = 9,
                                                               IsNested = false
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(Fakes.GenerateUpgradeRules());
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers(33));
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("BonusFeature");
        error.ErrorMessage.Should().Be("Bonus Feature '9' does not exist on Magical Power '33'");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldUpdateBonusFeature_WhenBonusFeatureUpgradeExistsAndOptionIsValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);
        
        character.Level = 2;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.bonusFeature,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x.UpgradeOption == UpgradeOption.bonusFeature).Id,
                                                  Block = 1,
                                                  Choice = JsonSerializer.Serialize(new BonusFeatureUpgrade
                                                           {
                                                               MagicalPowerId = 33,
                                                               BonusFeatureId = 2,
                                                               IsNested = false
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(rules);
        _upgradeRepository.UpsertUpgradesAsync(character.Id, Arg.Any<List<Upgrade>>()).Returns(true);
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers(33));
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());
        
        // Act
        bool result = await _sut.UpsertUpgradeAsync(update);

        // Assert
        result.Should().BeTrue();
        Upgrade? updatedUpgrade = character.Upgrades.FirstOrDefault(x => x.Option == UpgradeOption.bonusFeature);
        updatedUpgrade.Should().NotBeNull();

        try
        {
            ((BonusFeatureUpgrade)updatedUpgrade.Choice!).BonusFeatureId.Should().Be(2);
        }
        catch (Exception ex)
        {
            Assert.Fail("Could not parse upgrade Choice for BonusFeatureUpgrade");
        }
        
    }

    #endregion

    #region Talent

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenTalentUpgradeExistsAndTalentDoesNotExist()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);

        character.Level = 5;
        
        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = character.Id,
                                    UpgradeOption = UpgradeOption.talent,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x.UpgradeOption == UpgradeOption.talent).Id,
                                                  Block = 2,
                                                  Choice = JsonSerializer.Serialize(new GainTalentUpgrade
                                                           {
                                                               TalentId = 99
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(rules);
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers());
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("Talent");
        error.ErrorMessage.Should().Be("Talent '99' does not exist.");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldThrowException_WhenTalentUpgradeExistsAndTalentAlreadyExists()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = Guid.NewGuid(),
                                    UpgradeOption = UpgradeOption.talent,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x is { UpgradeOption: UpgradeOption.talent, Block: 2 }).Id,
                                                  Block = 2,
                                                  Choice = JsonSerializer.Serialize(new GainTalentUpgrade
                                                           {
                                                               TalentId = 22
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(Fakes.GenerateUpgradeRules());
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers(33));
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());

        // Act
        Func<Task<bool>> action = async () => await _sut.UpsertUpgradeAsync(update);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("TalentExists");
        error.ErrorMessage.Should().Be("Talent already present on character.");
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldUpdateTalent_WhenTalentUpgradeExistsAndOptionIsValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<UpgradeRule> rules = Fakes.GenerateUpgradeRules();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData().WithUpgrades(rules);

        character.Level = 5;

        UpgradeRequest update = new()
                                {
                                    AccountId = account.Id,
                                    CharacterId = Guid.NewGuid(),
                                    UpgradeOption = UpgradeOption.talent,
                                    Upgrade = new Upgrade
                                              {
                                                  Id = rules.First(x=>x.UpgradeOption == UpgradeOption.talent).Id,
                                                  Block = 2,
                                                  Choice = JsonSerializer.Serialize(new GainTalentUpgrade
                                                           {
                                                               TalentId = 43
                                                           })
                                              }
                                };

        _characterRepository.GetByIdAsync(update.CharacterId).Returns(character);
        _upgradeRepository.GetRulesAsync().Returns(rules);
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(Fakes.GenerateMagicalPowers(33));
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(Fakes.GenerateTalents());
        
        // Act
        bool result = await _sut.UpsertUpgradeAsync(update);

        // Assert
        result.Should().BeTrue();
        Upgrade? updatedUpgrade = character.Upgrades.FirstOrDefault(x => x.Option == UpgradeOption.talent);
        updatedUpgrade.Should().NotBeNull();

        try
        {
            ((GainTalentUpgrade)updatedUpgrade.Choice!).TalentId.Should().Be(43);
        }
        catch (Exception ex)
        {
            Assert.Fail("Could not parse upgrade Choice for GainTalentUpgrade");
        }
    }

    #endregion
}