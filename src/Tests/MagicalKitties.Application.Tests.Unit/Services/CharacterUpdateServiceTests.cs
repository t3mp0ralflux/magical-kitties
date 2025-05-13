using FluentAssertions;
using FluentValidation;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Application.Validators.Characters;
using NSubstitute;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class CharacterUpdateServiceTests
{
    private readonly ICharacterRepository _characterRepository = Substitute.For<ICharacterRepository>();
    private readonly ICharacterUpdateRepository _characterUpdateRepository = Substitute.For<ICharacterUpdateRepository>();
    private readonly IValidator<DescriptionUpdateValidationContext> _descriptionValidator = Substitute.For<IValidator<DescriptionUpdateValidationContext>>();
    private readonly IValidator<AttributeUpdateValidationContext> _updateValidator = Substitute.For<IValidator<AttributeUpdateValidationContext>>();

    public CharacterUpdateService _sut;

    public CharacterUpdateServiceTests()
    {
        _sut = new CharacterUpdateService(_characterRepository, _characterUpdateRepository, _descriptionValidator, _updateValidator);
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
        Enum.TryParse(typeof(DescriptionOption), "-1", out object? o);

        DescriptionUpdate update = new()
                                   {
                                       AccountId = Guid.NewGuid(),
                                       CharacterId = Guid.NewGuid()
                                   };

        _characterRepository.ExistsByIdAsync(update.CharacterId).Returns(true);

        // Act
        Func<Task<bool>> action = async () => await _sut.UpdateDescriptionAsync((DescriptionOption)o, update);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Selected description option is not valid");
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldCallUpdateNameAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        DescriptionUpdate update = new()
                                   {
                                       AccountId = Guid.NewGuid(),
                                       CharacterId = Guid.NewGuid(),
                                       Name = "Test"
                                   };

        _characterUpdateRepository.UpdateNameAsync(Arg.Any<DescriptionUpdate>()).Returns(true);
        _characterRepository.ExistsByIdAsync(update.CharacterId).Returns(true);

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.name, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.Received(1).UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateXPAsync(Arg.Any<DescriptionUpdate>());
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldCallUpdateDescriptionAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        DescriptionUpdate update = new()
                                   {
                                       AccountId = Guid.NewGuid(),
                                       CharacterId = Guid.NewGuid(),
                                       Description = "Test"
                                   };

        _characterUpdateRepository.UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>()).Returns(true);
        _characterRepository.ExistsByIdAsync(update.CharacterId).Returns(true);

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.description, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.DidNotReceive().UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.Received(1).UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateXPAsync(Arg.Any<DescriptionUpdate>());
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldCallUpdateHometownAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        DescriptionUpdate update = new()
                                   {
                                       AccountId = Guid.NewGuid(),
                                       CharacterId = Guid.NewGuid(),
                                       Hometown = "Test"
                                   };

        _characterUpdateRepository.UpdateHometownAsync(Arg.Any<DescriptionUpdate>()).Returns(true);
        _characterRepository.ExistsByIdAsync(update.CharacterId).Returns(true);

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.hometown, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.DidNotReceive().UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.Received(1).UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateXPAsync(Arg.Any<DescriptionUpdate>());
    }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldCallUpdateXPAndReturnTrue_WhenOptionIsSelected()
    {
        // Arrange
        DescriptionUpdate update = new()
                                   {
                                       AccountId = Guid.NewGuid(),
                                       CharacterId = Guid.NewGuid(),
                                       XP = 69
                                   };

        _characterUpdateRepository.UpdateXPAsync(Arg.Any<DescriptionUpdate>()).Returns(true);
        _characterRepository.ExistsByIdAsync(update.CharacterId).Returns(true);

        // Act
        bool result = await _sut.UpdateDescriptionAsync(DescriptionOption.xp, update);

        // Assert
        result.Should().BeTrue();

        await _characterUpdateRepository.DidNotReceive().UpdateNameAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateDescriptionAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.DidNotReceive().UpdateHometownAsync(Arg.Any<DescriptionUpdate>());
        await _characterUpdateRepository.Received(1).UpdateXPAsync(Arg.Any<DescriptionUpdate>());
    }
}