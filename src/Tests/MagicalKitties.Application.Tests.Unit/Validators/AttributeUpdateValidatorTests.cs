using FluentAssertions;
using FluentValidation;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Validators.Characters;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Validators;

public class AttributeUpdateValidatorTests
{
    public AttributeUpdateValidator _sut { get; set; }

    public AttributeUpdateValidatorTests()
    {
        _sut = new AttributeUpdateValidator();
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenAccountIdIsMissing()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(accountId: Guid.Empty);
        
        // Act
        Func<Task> action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenCharacterIdIsMissing()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(characterId: Guid.Empty);
        
        // Act
        Func<Task> action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(null, "Attribute must have a value")]
    [InlineData(int.MinValue, "Attribute cannot go below 0 or above 3")]
    [InlineData(int.MaxValue, "Attribute cannot go below 0 or above 3")]
    public async Task Validator_ShouldThrowAsync_WhenCuteIsSelectedAndValueIsInvalid(int? value, string exceptionMessage)
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(cute: value, attributeOption: AttributeOption.cute);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>(exceptionMessage);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenAttributeIsDuplicated()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(cute: 3, attributeOption: AttributeOption.cute);
        updateContext.Character.Cunning = 3;

        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Another attribute is assigned that value. Reduce that first and try again");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public async Task Validator_ShouldThrowAsync_WhenLevelIsInvalid(int? level)
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(level: level, attributeOption: AttributeOption.level);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Level can only be between 1 and 10 inclusively");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public async Task Validator_ShouldThrowAsync_WhenOwiesIsInvalid(int? owies)
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(owies: owies, attributeOption: AttributeOption.owies);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Owies can only be between 0 and 5 inclusively");
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenCurrentTreatsIsInvalid()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(currentTreats: -5, attributeOption: AttributeOption.currenttreats);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Current treats can't be negative");
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenFlawChangeIsNull()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(flawChange:null, attributeOption: AttributeOption.flaw);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Value cannot be null");
    }

    [Theory]
    [InlineData(19, 11, "Value of previous Id was out of range")]
    [InlineData(71, 11, "Value of previous Id was out of range")]
    [InlineData(-5, 11, "Value of previous Id was out of range")]
    [InlineData(21, 29, "Value of new Id was out of range")]
    [InlineData(21, 99, "Value of new Id was out of range")]
    [InlineData(21, -5, "Value of new Id was out of range")]
    public async Task Validator_ShouldThrowAsync_WhenFlawChangeIdsAreOutOfRange(int previousId, int newId, string exceptionMessage)
    {
        // Arrange
        var flawChange = new EndowmentChange()
                         {
                             NewId = newId,
                             PreviousId = previousId
                         };
        
        var updateContext = Fakes.GenerateValidationContext(flawChange:flawChange, attributeOption: AttributeOption.flaw);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>(exceptionMessage);
    }
    
    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenTalentChangeIsNull()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(talentChange:null, attributeOption: AttributeOption.talent);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Value cannot be null");
    }

    [Theory]
    [InlineData(19, 11, "Value of previous Id was out of range")]
    [InlineData(71, 11, "Value of previous Id was out of range")]
    [InlineData(-5, 11, "Value of previous Id was out of range")]
    [InlineData(21, 29, "Value of new Id was out of range")]
    [InlineData(21, 99, "Value of new Id was out of range")]
    [InlineData(21, -5, "Value of new Id was out of range")]
    public async Task Validator_ShouldThrowAsync_WhenTalentChangeIdsAreOutOfRange(int previousId, int newId, string exceptionMessage)
    {
        // Arrange
        var talentChange = new EndowmentChange()
                         {
                             NewId = newId,
                             PreviousId = previousId
                         };
        
        var updateContext = Fakes.GenerateValidationContext(talentChange:talentChange, attributeOption: AttributeOption.talent);
        updateContext.Character.Talents.Add(new Talent{Id = 66, Description = "Test", Name = "test", IsCustom = false});
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>(exceptionMessage);
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenDuplicateTalentIsFound()
    {
        // Arrange
        var talentChange = new EndowmentChange()
                           {
                               NewId = 21,
                               PreviousId = 21
                           };
        
        var updateContext = Fakes.GenerateValidationContext(talentChange:talentChange, attributeOption: AttributeOption.talent);
        
        updateContext.Character.Talents.Add(new Talent{Id = 21, Description = "Test", Name = "Test", IsCustom = false});

        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Talent already present on character. Choose another");
    }
    
    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenMagicalPowerChangeIsNull()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(magicalPowerChange:null, attributeOption: AttributeOption.magicalpower);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Value cannot be null");
    }

    [Theory]
    [InlineData(19, 11, "Value of previous Id was out of range")]
    [InlineData(71, 11, "Value of previous Id was out of range")]
    [InlineData(-5, 11, "Value of previous Id was out of range")]
    [InlineData(21, 29, "Value of new Id was out of range")]
    [InlineData(21, 99, "Value of new Id was out of range")]
    [InlineData(21, -5, "Value of new Id was out of range")]
    public async Task Validator_ShouldThrowAsync_WhenMagicalPowerChangeIdsAreOutOfRange(int previousId, int newId, string exceptionMessage)
    {
        // Arrange
        var magicalPowerChange = new EndowmentChange()
                         {
                             NewId = newId,
                             PreviousId = previousId
                         };
        
        var updateContext = Fakes.GenerateValidationContext(magicalPowerChange:magicalPowerChange, attributeOption: AttributeOption.magicalpower);
        updateContext.Character.MagicalPowers.Add(new MagicalPower{Id = 66, Name = "test", Description = "test", IsCustom = false});
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>(exceptionMessage);
    }
    
    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenMagicalPowerIsNotFound()
    {
        // Arrange
        var magicalPowerChange = new EndowmentChange()
                           {
                               NewId = 21,
                               PreviousId = 11
                           };
        
        var updateContext = Fakes.GenerateValidationContext(magicalPowerChange:magicalPowerChange, attributeOption: AttributeOption.magicalpower);

        updateContext.Character.MagicalPowers.Add(new MagicalPower{Id = 22, Description = "Test", Name = "Test", IsCustom = false, BonusFeatures = [new MagicalPower{Id = 21, Description = "Test", Name = "Test", IsCustom = false}]});
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Magical Power not present on character. Choose another");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validator_ShouldThrowAsync_WhenDuplicateMagicalPowerIsFound(bool isNestedPower)
    {
        // Arrange
        var magicalPowerChange = new EndowmentChange()
                           {
                               NewId = 21,
                               PreviousId = 21
                           };
        
        var updateContext = Fakes.GenerateValidationContext(magicalPowerChange:magicalPowerChange, attributeOption: AttributeOption.magicalpower);

        if (isNestedPower)
        {
            updateContext.Character.MagicalPowers.Add(new MagicalPower{Id = 22, Description = "Test", Name = "Test", IsCustom = false, BonusFeatures = [new MagicalPower{Id = 21, Description = "Test", Name = "Test", IsCustom = false}]});
        }
        else
        {
            updateContext.Character.MagicalPowers.Add(new MagicalPower{Id = 21, Description = "Test", Name = "Test", IsCustom = false});
        }

        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Magical Power already present on character. Choose another");
    }
    
    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenAttributeIsZero()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(cute: 0, attributeOption: AttributeOption.cute);

        updateContext.Character.Cunning = 0;

        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenCuteInformationIsCorrect()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(cute:3, attributeOption: AttributeOption.cute);
        
        updateContext.Character.Cunning = 0;
        updateContext.Character.Cute = 0;
        updateContext.Character.Fierce = 0;
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenCunningInformationIsCorrect()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(cunning:3, attributeOption: AttributeOption.cunning);
        
        updateContext.Character.Cunning = 0;
        updateContext.Character.Cute = 0;
        updateContext.Character.Fierce = 0;
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenFierceInformationIsCorrect()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(fierce:3, attributeOption: AttributeOption.fierce);
        
        updateContext.Character.Cunning = 0;
        updateContext.Character.Cute = 0;
        updateContext.Character.Fierce = 0;
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenLevelInformationIsCorrect()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(level:3, attributeOption: AttributeOption.level);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenOwiesInformationIsCorrect()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(owies:3, attributeOption: AttributeOption.owies);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenCurrentTreatsInformationIsCorrect()
    {
        // Arrange
        var updateContext = Fakes.GenerateValidationContext(currentTreats:3, attributeOption: AttributeOption.currenttreats);
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Validator_ShouldNotThrowAsync_WhenFlawInformationIsCorrect()
    {
        // Arrange
        var flawUpdate = new EndowmentChange()
                         {
                             PreviousId = 31,
                             NewId = 31
                         };

        var updateContext = Fakes.GenerateValidationContext(flawChange: flawUpdate, attributeOption: AttributeOption.flaw);

        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validator_ShouldNotThrowAsync_WhenTalentInformationIsCorrect(bool hasExisting)
    {
        // Arrange
        var talentUpdate = new EndowmentChange()
                         {
                             PreviousId = 41,
                             NewId = 31
                         };

        var updateContext = Fakes.GenerateValidationContext(talentChange: talentUpdate, attributeOption: AttributeOption.talent);

        if (hasExisting)
        {
            updateContext.Character.Talents.Add(new Talent{Id = 21, Description = "Test", IsCustom = false, Name = "TestTest"});
            updateContext.Character.Talents.Add(new Talent{Id = 41, Description = "Test", IsCustom = false, Name = "TestTest"});
        }
        
        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Validator_ShouldNotThrowAsync_WhenMagicalPowerInformationIsCorrect(bool hasNested)
    {
        // Arrange
        var magicalPowerUpdate = new EndowmentChange
                                 {
                                     PreviousId = 41,
                                     NewId = 31
                                 };
        var updateContext = Fakes.GenerateValidationContext(magicalPowerChange: magicalPowerUpdate, attributeOption: AttributeOption.magicalpower);

        if (hasNested)
        {
            updateContext.Character.MagicalPowers.Add(new MagicalPower{Id = 21, Name = "Test", Description = "test", BonusFeatures = [new MagicalPower{Id = 41, Name = "Test", Description = "test", IsCustom = false}], IsCustom = false});
        }

        // Act
        var action = async () => await _sut.ValidateAndThrowAsync(updateContext);

        // Assert
        await action.Should().NotThrowAsync();
    }
}