using FluentAssertions;
using FluentValidation;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Application.Validators.Characters;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class CharacterServiceTests
{
    private readonly ICharacterRepository _characterRepository = Substitute.For<ICharacterRepository>();
    private readonly IValidator<Character> _characterValidator = new CharacterValidator();
    private readonly IValidator<GetAllCharactersOptions> _optionsValidator = new GetAllCharactersOptionsValidator();

    public CharacterServiceTests()
    {
        _sut = new CharacterService(_characterRepository, _characterValidator, _optionsValidator);
    }

    public CharacterService _sut { get; set; }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenValidatorFails()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        Character character = Fakes.GenerateCharacter(account);
        character.Name = string.Empty;

        // Act
        Func<Task<bool>> action = async () => await _sut.CreateAsync(character);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFalse_WhenCharacterIsNotCreated()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.CreateAsync(Arg.Any<Character>()).Returns(false);

        // Act
        bool result = await _sut.CreateAsync(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenCharacterIsCreated()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.CreateAsync(Arg.Any<Character>()).Returns(true);

        // Act
        bool result = await _sut.CreateAsync(character);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenCharacterIsNotFound()
    {
        // Arrange
        _characterRepository.GetByIdAsync(Guid.NewGuid()).Returns((Character?)null);

        // Act
        Character? result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnCharacter_WhenCharacterIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.GetByIdAsync(character.Id).Returns(character);

        // Act
        Character? result = await _sut.GetByIdAsync(character.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(character);
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrowValidationException_WhenValidatorFails()
    {
        // Arrange
        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = Guid.NewGuid(),
                                              Page = 1,
                                              PageSize = 5,
                                              SortField = "stat"
                                          };
        // Act
        Func<Task<IEnumerable<Character>>> action = async () => await _sut.GetAllAsync(options);

        // Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoItemsAreFound()
    {
        // Arrange
        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = Guid.NewGuid(),
                                              Page = 1,
                                              PageSize = 5
                                          };

        _characterRepository.GetAllAsync(Arg.Any<GetAllCharactersOptions>()).Returns([]);

        // Act
        IEnumerable<Character> result = await _sut.GetAllAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnItems_WhenItemsAreFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = account.Id,
                                              Page = 1,
                                              PageSize = 5
                                          };

        List<Character> characters = Enumerable.Range(1, 5).Select(x => Fakes.GenerateCharacter(account)).ToList();

        _characterRepository.GetAllAsync(Arg.Any<GetAllCharactersOptions>()).Returns(characters);

        // Act
        IEnumerable<Character> result = await _sut.GetAllAsync(options);

        // Assert
        List<Character> resultList = result.ToList();

        resultList.Should().NotBeNullOrEmpty();
        resultList.Should().BeEquivalentTo(characters);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task GetCountAsync_ShouldReturnCorrectCount_WhenItemsAreFoundOrNot(int count)
    {
        // Arrange
        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = Guid.NewGuid(),
                                              Page = 1,
                                              PageSize = 5
                                          };

        _characterRepository.GetCountAsync(Arg.Any<GetAllCharactersOptions>()).Returns(count);

        // Act
        int result = await _sut.GetCountAsync(options);

        // Assert
        result.Should().Be(count);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.ExistsByIdAsync(character.Id).Returns(false);

        // Act
        bool result = await _sut.DeleteAsync(character.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenDatabaseFailsToDelete()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.ExistsByIdAsync(character.Id).Returns(true);
        _characterRepository.DeleteAsync(character.Id).Returns(false);

        // Act
        bool result = await _sut.DeleteAsync(character.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDatabaseDeletesCharacter()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.ExistsByIdAsync(character.Id).Returns(true);
        _characterRepository.DeleteAsync(character.Id).Returns(true);

        // Act
        bool result = await _sut.DeleteAsync(character.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CopyAsync_ShouldReturnCopiedCharacter_WhenCharacterIsCopied()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account)
                                   .WithBaselineData()
                                   .WithHumanData()
                                   .WithUpgrades(Fakes.GenerateUpgradeRules());

        _characterRepository.GetByIdAsync(character.Id).Returns(character);
        _characterRepository.CopyAsync(Arg.Any<Character>()).Returns(true);
        
        // Act
        Character result = await _sut.CopyAsync(character.Id);

        // Assert
        result.Name.Should().Be($"{character.Name} - Copy");
        result.Id.Should().NotBe(character.Id);
        
        // Excludes are specifically checked next
        result.Should().BeEquivalentTo(character, options =>
                                                  {
                                                      options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>();
                                                      options.Excluding(x => x.Id);
                                                      options.Excluding(x => x.Name);
                                                      options.Excluding(x => x.Humans);

                                                      return options;
                                                  });
        
        foreach (Human resultHuman in result.Humans)
        {
            Human? humanMatch = character.Humans.FirstOrDefault(x => x.Name == resultHuman.Name);
            
            humanMatch.Should().NotBeNull();
            resultHuman.Should().BeEquivalentTo(humanMatch, options =>
                                                       {
                                                           options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>();
                                                           options.Excluding(x => x.Id);
                                                           options.Excluding(x => x.CharacterId);
                                                           options.Excluding(x => x.Problems);
                                                           return options;
                                                       });

            resultHuman.CharacterId.Should().Be(result.Id);
            foreach (Problem resultHumanProblem in resultHuman.Problems)
            {
                Problem? problemMatch = humanMatch.Problems.FirstOrDefault(x => x.Emotion == resultHumanProblem.Emotion);
                
                problemMatch.Should().NotBeNull();
                resultHumanProblem.Should().BeEquivalentTo(problemMatch, options =>
                                                                         {
                                                                             options.Excluding(x => x.Id);
                                                                             options.Excluding(x => x.HumanId);

                                                                             return options;
                                                                         });

                resultHumanProblem.HumanId.Should().Be(resultHuman.Id);
            }
        }
    }
}