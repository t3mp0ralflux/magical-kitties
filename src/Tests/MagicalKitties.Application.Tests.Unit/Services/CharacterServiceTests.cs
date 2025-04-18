using FluentAssertions;
using FluentValidation;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Application.Validators.Characters;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class CharacterServiceTests
{
    private readonly ICharacterRepository _characterRepository = Substitute.For<ICharacterRepository>();
    private readonly IValidator<Character> _characterValidator = new CharacterValidator();
    private readonly IFlawRepository _flawRepository = Substitute.For<IFlawRepository>();
    private readonly ILogger<CharacterService> _logger = Substitute.For<ILogger<CharacterService>>();
    private readonly IValidator<GetAllCharactersOptions> _optionsValidator = new GetAllCharactersOptionsValidator();
    private readonly ITalentRepository _talentRepository = Substitute.For<ITalentRepository>();

    public CharacterServiceTests()
    {
        _sut = new CharacterService(_characterRepository, _characterValidator, _optionsValidator, _logger, _flawRepository, _talentRepository);
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
    public async Task UpdateAsync_ShouldReturnFalse_WhenAccountIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.ExistsByIdAsync(character.Id).Returns(false);

        // Act
        bool result = await _sut.UpdateAsync(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenDatabaseFailsToUpdate()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.ExistsByIdAsync(character.Id).Returns(true);
        _characterRepository.UpdateAsync(Arg.Any<Character>()).Returns(false);

        // Act
        bool result = await _sut.UpdateAsync(character);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenCharacterIsUpdated()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _characterRepository.ExistsByIdAsync(character.Id).Returns(true);
        _characterRepository.UpdateAsync(Arg.Any<Character>()).Returns(true);

        // Act
        bool result = await _sut.UpdateAsync(character);

        // Assert
        result.Should().BeTrue();
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
}