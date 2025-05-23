﻿using FluentValidation;
using FluentValidation.TestHelper;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Validators.Characters;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Validators;

public class AttributeUpdateValidatorTests
{
    public AttributeUpdateValidatorTests()
    {
        _sut = new AttributeUpdateValidator();
    }

    public AttributeUpdateValidator _sut { get; set; }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenAccountIdIsMissing()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(Guid.Empty);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Update.AccountId);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenCharacterIdIsMissing()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(characterId: Guid.Empty);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Update.CharacterId);
    }

    [Theory]
    [InlineData(null, "'Update Cute' must not be empty.")]
    [InlineData(int.MinValue, "Attribute can only be between 0 and 3 inclusively.")]
    [InlineData(int.MaxValue, "Attribute can only be between 0 and 3 inclusively.")]
    public async Task Validator_ShouldThrowAsync_WhenCuteIsSelectedAndValueIsInvalid(int? value, string exceptionMessage)
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(cute: value, attributeOption: AttributeOption.cute);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(nameof(AttributeUpdate.Cute))
            .WithErrorMessage(exceptionMessage)
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenAttributeIsDuplicated()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(cute: 3, attributeOption: AttributeOption.cute);
        updateContext.Character.Cunning = 3;

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(nameof(AttributeUpdate.Cute))
            .WithErrorMessage("Another attribute is assigned that value. Reduce that first and try again.")
            .WithSeverity(Severity.Error);
    }

    [Theory]
    [InlineData(null, "'Update Level' must not be empty.")]
    [InlineData(int.MinValue, "Level can only be between 1 and 10 inclusively.")]
    [InlineData(int.MaxValue, "Level can only be between 1 and 10 inclusively.")]
    public async Task Validator_ShouldThrowAsync_WhenLevelIsInvalid(int? level, string exceptionMessage)
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(level: level, attributeOption: AttributeOption.level);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Update.Level)
            .WithErrorMessage(exceptionMessage)
            .WithSeverity(Severity.Error);
    }

    [Theory]
    [InlineData(null, "'Update Current Owies' must not be empty.")]
    [InlineData(int.MinValue, "Owies can only be between 0 and 5 inclusively.")]
    [InlineData(int.MaxValue, "Owies can only be between 0 and 5 inclusively.")]
    public async Task Validator_ShouldThrowAsync_WhenOwiesIsInvalid(int? owies, string exceptionMessage)
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(currentOwies: owies, attributeOption: AttributeOption.currentowies);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Update.CurrentOwies)
            .WithErrorMessage(exceptionMessage)
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenCurrentTreatsIsInvalid()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(currentTreats: -5, attributeOption: AttributeOption.currenttreats);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Update.CurrentTreats)
            .WithErrorMessage("Current Treats can't be negative.")
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenCurrentInjuriesIsInvalid()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(currentInjuries: -5, attributeOption: AttributeOption.currentinjuries);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Update.CurrentInjuries)
            .WithErrorMessage("Current Injuries can't be negative.")
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenFlawChangeIsNull()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(flawChange: null, attributeOption: AttributeOption.flaw);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(nameof(AttributeUpdate.FlawChange))
            .WithErrorMessage("'Update FlawChange' cannot be null.")
            .WithSeverity(Severity.Error);
    }

    [Theory]
    [InlineData(19, 11, "PreviousId", "Value of FlawChange {0} was out of range.")]
    [InlineData(71, 11, "PreviousId", "Value of FlawChange {0} was out of range.")]
    [InlineData(-5, 11, "PreviousId", "Value of FlawChange {0} was out of range.")]
    [InlineData(21, 29, "NewId", "Value of FlawChange {0} was out of range.")]
    [InlineData(21, 99, "NewId", "Value of FlawChange {0} was out of range.")]
    [InlineData(21, -5, "NewId", "Value of FlawChange {0} was out of range.")]
    public async Task Validator_ShouldThrowAsync_WhenFlawChangeIdsAreOutOfRange(int previousId, int newId, string fieldName, string exceptionMessage)
    {
        // Arrange
        EndowmentChange flawChange = new()
                                     {
                                         NewId = newId,
                                         PreviousId = previousId
                                     };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(flawChange: flawChange, attributeOption: AttributeOption.flaw);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor($"{nameof(AttributeUpdate.FlawChange)}.{fieldName}")
            .WithErrorMessage(string.Format(exceptionMessage, fieldName))
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenTalentChangeIsNull()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(talentChange: null, attributeOption: AttributeOption.talent);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(nameof(AttributeUpdate.TalentChange))
            .WithErrorMessage("'Update TalentChange' cannot be null.")
            .WithSeverity(Severity.Error);
    }

    [Theory]
    [InlineData(19, 11, "PreviousId", "Value of TalentChange {0} was out of range.")]
    [InlineData(71, 11, "PreviousId", "Value of TalentChange {0} was out of range.")]
    [InlineData(-5, 11, "PreviousId", "Value of TalentChange {0} was out of range.")]
    [InlineData(21, 29, "NewId", "Value of TalentChange {0} was out of range.")]
    [InlineData(21, 99, "NewId", "Value of TalentChange {0} was out of range.")]
    [InlineData(21, -5, "NewId", "Value of TalentChange {0} was out of range.")]
    public async Task Validator_ShouldThrowAsync_WhenTalentChangeIdsAreOutOfRange(int previousId, int newId, string fieldName, string exceptionMessage)
    {
        // Arrange
        EndowmentChange talentChange = new()
                                       {
                                           NewId = newId,
                                           PreviousId = previousId
                                       };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(talentChange: talentChange, attributeOption: AttributeOption.talent);
        updateContext.Character.Talents.Add(new Talent { Id = 66, Description = "Test", Name = "test", IsCustom = false });

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor($"{nameof(AttributeUpdate.TalentChange)}.{fieldName}")
            .WithErrorMessage(string.Format(exceptionMessage, fieldName))
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenDuplicateTalentIsFound()
    {
        // Arrange
        EndowmentChange talentChange = new()
                                       {
                                           NewId = 21,
                                           PreviousId = 21
                                       };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(talentChange: talentChange, attributeOption: AttributeOption.talent);

        updateContext.Character.Talents.Add(new Talent { Id = 21, Description = "Test", Name = "Test", IsCustom = false });

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor("TalentChange.NewId")
            .WithErrorMessage("Talent '21' already present on character. Choose another.")
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenTalentIsAddedAndCharacterIsNotTheRightLevel()
    {
        // Arrange
        EndowmentChange talentChange = new()
                                       {
                                           NewId = 21,
                                           PreviousId = 21
                                       };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(talentChange: talentChange, attributeOption: AttributeOption.talent);
        updateContext.Character.Level = 4;
        updateContext.Character.Talents.Add(new Talent { Id = 11, Name = "Test", Description = "TestTEst", IsCustom = false });

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor("TalentChange.NewId")
            .WithErrorMessage("Character is not level 5 or above. Cannot add new Talent.")
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenTalentIsAddedAndCharacterAlreadyHasMax()
    {
        // Arrange
        EndowmentChange talentChange = new()
                                       {
                                           NewId = 21,
                                           PreviousId = 21
                                       };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(talentChange: talentChange, attributeOption: AttributeOption.talent);
        updateContext.Character.Level = 6;
        updateContext.Character.Talents.Add(new Talent { Id = 11, Name = "Test", Description = "TestTEst", IsCustom = false });
        updateContext.Character.Talents.Add(new Talent { Id = 31, Name = "Test", Description = "TestTEst", IsCustom = false });

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor("TalentChange.NewId")
            .WithErrorMessage("Characters cannot have more than two Talents.")
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenMagicalPowerChangeIsNull()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(magicalPowerChange: null, attributeOption: AttributeOption.magicalpower);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(nameof(AttributeUpdate.MagicalPowerChange))
            .WithErrorMessage("'Update MagicalPowerChange' cannot be null.")
            .WithSeverity(Severity.Error);
    }

    [Theory]
    [InlineData(19, 11, "PreviousId", "Value of MagicalPowerChange {0} was out of range.")]
    [InlineData(71, 11, "PreviousId", "Value of MagicalPowerChange {0} was out of range.")]
    [InlineData(-5, 11, "PreviousId", "Value of MagicalPowerChange {0} was out of range.")]
    [InlineData(21, 29, "NewId", "Value of MagicalPowerChange {0} was out of range.")]
    [InlineData(21, 99, "NewId", "Value of MagicalPowerChange {0} was out of range.")]
    [InlineData(21, -5, "NewId", "Value of MagicalPowerChange {0} was out of range.")]
    public async Task Validator_ShouldThrowAsync_WhenMagicalPowerChangeIdsAreOutOfRange(int previousId, int newId, string fieldName, string exceptionMessage)
    {
        // Arrange
        EndowmentChange magicalPowerChange = new()
                                             {
                                                 NewId = newId,
                                                 PreviousId = previousId
                                             };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(magicalPowerChange: magicalPowerChange, attributeOption: AttributeOption.magicalpower);
        updateContext.Character.MagicalPowers.Add(new MagicalPower { Id = 66, Name = "test", Description = "test", IsCustom = false });

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor($"{nameof(AttributeUpdate.MagicalPowerChange)}.{fieldName}")
            .WithErrorMessage(string.Format(exceptionMessage, fieldName))
            .WithSeverity(Severity.Error);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validator_ShouldThrowAsync_WhenDuplicateMagicalPowerIsFound(bool isNestedPower)
    {
        // Arrange
        EndowmentChange magicalPowerChange = new()
                                             {
                                                 NewId = 21,
                                                 PreviousId = 21
                                             };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(magicalPowerChange: magicalPowerChange, attributeOption: AttributeOption.magicalpower);

        if (isNestedPower)
        {
            updateContext.Character.MagicalPowers.Add(new MagicalPower { Id = 22, Description = "Test", Name = "Test", IsCustom = false, BonusFeatures = [new MagicalPower { Id = 21, Description = "Test", Name = "Test", IsCustom = false }] });
        }
        else
        {
            updateContext.Character.MagicalPowers.Add(new MagicalPower { Id = 21, Description = "Test", Name = "Test", IsCustom = false });
        }

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor("MagicalPowerChange.NewId")
            .WithErrorMessage("Magical Power '21' already present on character. Choose another.")
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenMagicalPowerIsAddedAndCharacterIsNotTheRightLevel()
    {
        // Arrange
        EndowmentChange magicalPowerChange = new()
                                             {
                                                 NewId = 21,
                                                 PreviousId = 21
                                             };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(magicalPowerChange: magicalPowerChange, attributeOption: AttributeOption.magicalpower);
        updateContext.Character.Level = 7;
        updateContext.Character.MagicalPowers.Add(new MagicalPower { Id = 22, Name = "Test", Description = "TestTest", IsCustom = false, BonusFeatures = [] });

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor("MagicalPowerChange.NewId")
            .WithErrorMessage("Character is not level 8 or above or Undead. Cannot add new Magical Power.")
            .WithSeverity(Severity.Error);
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenAttributeIsZero()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(cute: 0, attributeOption: AttributeOption.cute);

        updateContext.Character.Cunning = 0;

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenCuteInformationIsCorrect()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(cute: 3, attributeOption: AttributeOption.cute);

        updateContext.Character.Cunning = 0;
        updateContext.Character.Cute = 0;
        updateContext.Character.Fierce = 0;

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenCunningInformationIsCorrect()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(cunning: 3, attributeOption: AttributeOption.cunning);

        updateContext.Character.Cunning = 0;
        updateContext.Character.Cute = 0;
        updateContext.Character.Fierce = 0;

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenFierceInformationIsCorrect()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(fierce: 3, attributeOption: AttributeOption.fierce);

        updateContext.Character.Cunning = 0;
        updateContext.Character.Cute = 0;
        updateContext.Character.Fierce = 0;

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenLevelInformationIsCorrect()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(level: 3, attributeOption: AttributeOption.level);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenOwiesInformationIsCorrect()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(currentOwies: 3, attributeOption: AttributeOption.currentowies);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenCurrentTreatsInformationIsCorrect()
    {
        // Arrange
        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(currentTreats: 3, attributeOption: AttributeOption.currenttreats);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenFlawInformationIsCorrect()
    {
        // Arrange
        EndowmentChange flawUpdate = new()
                                     {
                                         PreviousId = 31,
                                         NewId = 31
                                     };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(flawChange: flawUpdate, attributeOption: AttributeOption.flaw);

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validator_ShouldNotThrowAsync_WhenTalentInformationIsCorrect(bool hasExisting)
    {
        // Arrange
        EndowmentChange talentUpdate = new()
                                       {
                                           PreviousId = 41,
                                           NewId = 31
                                       };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(talentChange: talentUpdate, attributeOption: AttributeOption.talent);

        if (hasExisting)
        {
            updateContext.Character.Talents.Add(new Talent { Id = 21, Description = "Test", IsCustom = false, Name = "TestTest" });
            updateContext.Character.Talents.Add(new Talent { Id = 41, Description = "Test", IsCustom = false, Name = "TestTest" });
        }

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validator_ShouldNotThrowAsync_WhenMagicalPowerInformationIsCorrect(bool hasNested)
    {
        // Arrange
        EndowmentChange magicalPowerUpdate = new()
                                             {
                                                 NewId = 31,
                                                 PreviousId = 41
                                             };

        AttributeUpdateValidationContext updateContext = Fakes.GenerateValidationContext(magicalPowerChange: magicalPowerUpdate, attributeOption: AttributeOption.magicalpower);

        if (hasNested)
        {
            updateContext.Character.MagicalPowers.Add(new MagicalPower { Id = 21, Name = "Test", Description = "test", BonusFeatures = [new MagicalPower { Id = 41, Name = "Test", Description = "test", IsCustom = false }], IsCustom = false });
        }
        else
        {
            updateContext.Character.MagicalPowers.Add(new MagicalPower { Id = 41, Name = "Test", Description = "Test", IsCustom = false, BonusFeatures = [] });
        }

        // Act
        TestValidationResult<AttributeUpdateValidationContext>? result = await _sut.TestValidateAsync(updateContext);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}