using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.Characters.Upgrades;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Application.Validators.Characters;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class CharacterUpdateServiceTests
{
    private readonly ICharacterRepository _characterRepository = Substitute.For<ICharacterRepository>();
    private readonly ICharacterUpdateRepository _characterUpdateRepository = Substitute.For<ICharacterUpdateRepository>();
    private readonly IUpgradeRepository _upgradeRepository = Substitute.For<IUpgradeRepository>();
    private readonly IValidator<DescriptionUpdateValidationContext> _descriptionValidator = Substitute.For<IValidator<DescriptionUpdateValidationContext>>();
    private readonly IValidator<AttributeUpdateValidationContext> _updateValidator = Substitute.For<IValidator<AttributeUpdateValidationContext>>();

    public CharacterUpdateService _sut;

    public CharacterUpdateServiceTests()
    {
        _sut = new CharacterUpdateService(_characterRepository, _characterUpdateRepository, _descriptionValidator, _updateValidator, _upgradeRepository);
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldReturnFalse_WhenCharacterDoesNotExist()
    {
        // Arrange
        DescriptionUpdate update = new()
                                   {
                                       AccountId = Guid.NewGuid(),
                                       CharacterId = Guid.NewGuid()
                                   };

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.name, update);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldThrowValidationException_WhenDescriptionOptionIsNotValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Enum.TryParse(typeof(DescriptionOption), "-1", out object? o);

        DescriptionUpdate update = new()
                                   {
                                       AccountId = account.Id,
                                       CharacterId = Guid.NewGuid()
                                   };

        _characterRepository.ExistsByIdAsync(account.Id, update.CharacterId).Returns(true);

        // Act
        Func<Task<bool>> action = async () => await _sut.UpdateDescriptionAsync((DescriptionOption)o, update);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Selected description option is not valid");
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldCallUpdateNameAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        DescriptionUpdate update = new()
                                   {
                                       AccountId = account.Id,
                                       CharacterId = Guid.NewGuid(),
                                       Name = "Test"
                                   };

        _characterUpdateRepository.UpdateNameAsync(Arg.Any<DescriptionUpdate>()).Returns(true);
        _characterRepository.ExistsByIdAsync(account.Id, update.CharacterId).Returns(true);

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.name, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.Received(1).UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldCallUpdateDescriptionAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        DescriptionUpdate update = new()
                                   {
                                       AccountId = account.Id,
                                       CharacterId = Guid.NewGuid(),
                                       Description = "Test"
                                   };

        _characterUpdateRepository.UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>()).Returns(true);
        _characterRepository.ExistsByIdAsync(account.Id, update.CharacterId).Returns(true);

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.description, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.DidNotReceive().UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.Received(1).UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldCallUpdateHometownAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        DescriptionUpdate update = new()
                                   {
                                       AccountId = account.Id,
                                       CharacterId = Guid.NewGuid(),
                                       Hometown = "Test"
                                   };

        _characterUpdateRepository.UpdateHometownAsync(Arg.Any<DescriptionUpdate>()).Returns(true);
        _characterRepository.ExistsByIdAsync(account.Id, update.CharacterId).Returns(true);

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.hometown, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.DidNotReceive().UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.Received(1).UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
    }

    [Fact]
    public async Task UpdateAttributeAsync_ShouldCallUpdateXPAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        AttributeUpdate update = new()
                                 {
                                     AccountId = account.Id,
                                     Character = character,
                                     XP = 69
                                 };

        _characterUpdateRepository.UpdateXPAsync(Arg.Any<AttributeUpdate>()).Returns(true);
        _characterRepository.GetByIdAsync(account.Id, update.Character.Id).Returns(character);

        // Act
        bool result = await _sut.UpdateAttributeAsync(AttributeOption.xp, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.DidNotReceive().UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.Received(1).UpdateXPAsync(Arg.Any<AttributeUpdate>());
    }

    [Fact]
    public async Task UpdateAttributeAsync_ShouldUpdateTalentAndRemoveRelevantUpgrades_WhenTalentIsChanged()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        List<Talent> fakeTalents = Fakes.GenerateTalents();

        Upgrade characterUpgrade = new Upgrade
                                   {
                                       Id = Guid.NewGuid(),
                                       Block = 2,
                                       Option = UpgradeOption.talent,
                                       Choice = JsonSerializer.Serialize(new GainTalentUpgrade
                                                                         {
                                                                             TalentId = fakeTalents[0].Id
                                                                         })
                                   };

        character.Talents.Add(fakeTalents[0]);
        character.Upgrades.Add(characterUpgrade);

        AttributeUpdate update = new AttributeUpdate
                                 {
                                     AccountId = account.Id,
                                     Character = character,
                                     TalentChange = new EndowmentChange
                                                    {
                                                        NewId = fakeTalents[1].Id,
                                                        PreviousId = fakeTalents[0].Id,
                                                        IsPrimary = true
                                                    }
                                 };

        _characterRepository.GetByIdAsync(account.Id, character.Id).Returns(character);
        _characterUpdateRepository.UpdateTalentAsync(Arg.Any<AttributeUpdate>()).Returns(true);
        _upgradeRepository.UpsertUpgradesAsync(Arg.Any<Guid>(), Arg.Any<List<Upgrade>>(), Arg.Any<CancellationToken>()).Returns(true);
        
        // Act
        await _sut.UpdateAttributeAsync(AttributeOption.talent, update);

        // Assert
        character.Upgrades[0].Choice.Should().BeNull();
    }
}