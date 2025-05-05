using FluentAssertions;
using FluentValidation;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Validators.Characters;
using NSubstitute;

namespace MagicalKitties.Application.Tests.Unit.Validators;

public class CharacterDescriptionUpdateValidatorTests
{
    public DescriptionUpdateValidator _sut;

    public CharacterDescriptionUpdateValidatorTests()
    {
        _sut = new DescriptionUpdateValidator();
    }

    [Fact]
    public async Task Validator_ShouldThrowError_WhenNameIsMissing()
    {
        // Arrange
        var descriptionUpdate = new DescriptionUpdate()
                                {
                                    DescriptionOption = DescriptionOption.name,
                                    AccountId = Guid.NewGuid(),
                                    CharacterId = Guid.NewGuid()
                                };
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(descriptionUpdate);

        // Assert
        var result = await action.Should().ThrowAsync<ValidationException>();

        var errors = result.Subject.FirstOrDefault()?.Errors.ToList();
        errors.Should().ContainSingle();

        var error = errors.First();
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("Name cannot be empty");
    }
    
    [Fact]
    public async Task Validator_ShouldThrowError_WhenXPIsMissing()
    {
        // Arrange
        var descriptionUpdate = new DescriptionUpdate()
                                {
                                    DescriptionOption = DescriptionOption.xp,
                                    AccountId = Guid.NewGuid(),
                                    CharacterId = Guid.NewGuid()
                                };
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(descriptionUpdate);

        // Assert
        var result = await action.Should().ThrowAsync<ValidationException>();

        var errors = result.Subject.FirstOrDefault()?.Errors.ToList();
        errors.Should().ContainSingle();

        var error = errors.First();
        error.PropertyName.Should().Be("XP");
        error.ErrorMessage.Should().Be("XP cannot be empty");
    }
    
    [Fact]
    public async Task Validator_ShouldThrowError_WhenXPIsNegative()
    {
        // Arrange
        var descriptionUpdate = new DescriptionUpdate()
                                {
                                    DescriptionOption = DescriptionOption.xp,
                                    AccountId = Guid.NewGuid(),
                                    CharacterId = Guid.NewGuid(),
                                    XP = -42069
                                };
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(descriptionUpdate);

        // Assert
        var result = await action.Should().ThrowAsync<ValidationException>();

        var errors = result.Subject.FirstOrDefault()?.Errors.ToList();
        errors.Should().ContainSingle();

        var error = errors.First();
        error.PropertyName.Should().Be("XP");
        error.ErrorMessage.Should().Be("XP cannot be negative");
    }
    
    [Fact]
    public async Task Validator_ShouldThrowError_WhenXPIsHuge()
    {
        // Arrange
        var descriptionUpdate = new DescriptionUpdate()
                                {
                                    DescriptionOption = DescriptionOption.xp,
                                    AccountId = Guid.NewGuid(),
                                    CharacterId = Guid.NewGuid(),
                                    XP = int.MaxValue
                                };
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(descriptionUpdate);

        // Assert
        var result = await action.Should().ThrowAsync<ValidationException>();

        var errors = result.Subject.FirstOrDefault()?.Errors.ToList();
        errors.Should().ContainSingle();

        var error = errors.First();
        error.PropertyName.Should().Be("XP");
        error.ErrorMessage.Should().Be("XP value exceeds game capacity");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validator_ShouldNotThrowError_WhenDescriptionIsNullOrWhitespace(string? description)
    {
        // Arrange
        var descriptionUpdate = new DescriptionUpdate()
                                {
                                    DescriptionOption = DescriptionOption.description,
                                    AccountId = Guid.NewGuid(),
                                    CharacterId = Guid.NewGuid(),
                                    Description = description
                                };
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(descriptionUpdate);

        // Assert
        await action.Should().NotThrowAsync<ValidationException>();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validator_ShouldNotThrowError_WhenHometownIsNullOrWhitespace(string? hometown)
    {
        // Arrange
        var descriptionUpdate = new DescriptionUpdate()
                                {
                                    DescriptionOption = DescriptionOption.description,
                                    AccountId = Guid.NewGuid(),
                                    CharacterId = Guid.NewGuid(),
                                    Hometown = hometown
                                };
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(descriptionUpdate);

        // Assert
        await action.Should().NotThrowAsync<ValidationException>();
    }
}