using FluentAssertions;
using FluentAssertions.Specialized;
using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Validators.Characters;

namespace MagicalKitties.Application.Tests.Unit.Validators;

public class CharacterDescriptionUpdateValidatorTests
{
    public readonly DescriptionUpdateValidator _sut;

    public CharacterDescriptionUpdateValidatorTests()
    {
        _sut = new DescriptionUpdateValidator();
    }

    [Fact]
    public async Task Validator_ShouldThrowError_WhenNameIsMissing()
    {
        // Arrange
        DescriptionUpdate descriptionUpdate = new()
                                              {
                                                  AccountId = Guid.NewGuid(),
                                                  CharacterId = Guid.NewGuid()
                                              };

        DescriptionUpdateValidationContext validationContext = new()
                                                               {
                                                                   Option = DescriptionOption.name,
                                                                   Update = descriptionUpdate
                                                               };
        // Act
        Func<Task> action = async () => await _sut.ValidateAndThrowAsync(validationContext);

        // Assert
        ExceptionAssertions<ValidationException>? result = await action.Should().ThrowAsync<ValidationException>();

        List<ValidationFailure>? errors = result.Subject.FirstOrDefault()?.Errors.ToList();
        errors.Should().ContainSingle();

        ValidationFailure error = errors.First();
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("Name cannot be empty");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validator_ShouldNotThrowError_WhenDescriptionIsNullOrWhitespace(string? description)
    {
        // Arrange
        DescriptionUpdate descriptionUpdate = new()
                                              {
                                                  AccountId = Guid.NewGuid(),
                                                  CharacterId = Guid.NewGuid(),
                                                  Description = description
                                              };

        DescriptionUpdateValidationContext validationContext = new()
                                                               {
                                                                   Option = DescriptionOption.description,
                                                                   Update = descriptionUpdate
                                                               };

        // Act
        Func<Task> action = async () => await _sut.ValidateAndThrowAsync(validationContext);

        // Assert
        await action.Should().NotThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validator_ShouldNotThrowError_WhenHometownIsNullOrWhitespace(string? hometown)
    {
        // Arrange
        DescriptionUpdate descriptionUpdate = new()
                                              {
                                                  AccountId = Guid.NewGuid(),
                                                  CharacterId = Guid.NewGuid(),
                                                  Hometown = hometown
                                              };

        DescriptionUpdateValidationContext validationContext = new()
                                                               {
                                                                   Option = DescriptionOption.hometown,
                                                                   Update = descriptionUpdate
                                                               };

        // Act
        Func<Task> action = async () => await _sut.ValidateAndThrowAsync(validationContext);

        // Assert
        await action.Should().NotThrowAsync<ValidationException>();
    }
}