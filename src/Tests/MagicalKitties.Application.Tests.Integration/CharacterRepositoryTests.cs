using System.Data;
using Dapper;
using FluentAssertions;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Repositories.Implementation;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Integration;

public class CharacterRepositoryTests : IClassFixture<ApplicationApiFactory>
{
    private readonly AccountRepository _accountRepository;
    private readonly CharacterUpdateRepository _characterUpdateRepository;
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    public CharacterRepositoryTests(ApplicationApiFactory apiFactory)
    {
        IDbConnectionFactory dbConnectionFactory = apiFactory.Services.GetRequiredService<IDbConnectionFactory>();

        _sut = new CharacterRepository(dbConnectionFactory, _dateTimeProvider);
        _accountRepository = new AccountRepository(dbConnectionFactory, _dateTimeProvider);
        _characterUpdateRepository = new CharacterUpdateRepository(dbConnectionFactory, _dateTimeProvider);
    }

    public CharacterRepository _sut { get; set; }

    [SkipIfEnvironmentMissingFact]
    public async Task CreateAsync_ShouldCreateCharacterWithoutCharacteristics_WhenNewCharacterIsCreated()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateNewCharacter(account);

        await _accountRepository.CreateAsync(account);

        // Act
        bool result = await _sut.CreateAsync(character);

        // Assert
        result.Should().BeTrue();

        Character? createdCharacter = await _sut.GetByIdAsync(account.Id, character.Id);
        createdCharacter.Should().NotBeNull();
        createdCharacter.Should().BeEquivalentTo(character);
    }

    [SkipIfEnvironmentMissingFact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        // Act
        Character? result = await _sut.GetByIdAsync(account.Id,Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [SkipIfEnvironmentMissingFact]
    public async Task GetByIdAsync_ShouldReturnBasicCharacter_WhenIdIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        DateTime now = DateTime.UtcNow;
        _dateTimeProvider.GetUtcNow().Returns(now);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        // Act
        Character? result = await _sut.GetByIdAsync(account.Id, character.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(character, options => options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    [SkipIfEnvironmentMissingFact]
    public async Task GetByIdAsync_ShouldReturnFullCharacterInformation_WhenIdIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account).WithBaselineData();

        DateTime now = DateTime.UtcNow;
        _dateTimeProvider.GetUtcNow().Returns(now);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        AttributeUpdate cunningUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.cunning);
        AttributeUpdate cuteUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.cute);
        AttributeUpdate fierceUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.fierce);
        AttributeUpdate levelUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.level);
        AttributeUpdate flawUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.flaw);
        AttributeUpdate talentUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.talent);
        AttributeUpdate magicalPowerUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.magicalpower);
        AttributeUpdate currentInjuriesUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.currentinjuries);
        AttributeUpdate currentOwiesUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.currentowies);
        AttributeUpdate currentTreatsUpdate = Fakes.GenerateAttributeUpdate(account.Id, character.Id, AttributeOption.currenttreats);

        await _characterUpdateRepository.UpdateLevelAsync(levelUpdate);
        await _characterUpdateRepository.UpdateCunningAsync(cunningUpdate);
        await _characterUpdateRepository.UpdateCuteAsync(cuteUpdate);
        await _characterUpdateRepository.UpdateFierceAsync(fierceUpdate);
        
        await _characterUpdateRepository.CreateFlawAsync(flawUpdate);
        await _characterUpdateRepository.CreateTalentAsync(talentUpdate);
        await _characterUpdateRepository.CreateMagicalPowerAsync(magicalPowerUpdate);

        await _characterUpdateRepository.UpdateCurrentInjuriesAsync(currentInjuriesUpdate);
        await _characterUpdateRepository.UpdateCurrentOwiesAsync(currentOwiesUpdate);
        await _characterUpdateRepository.UpdateCurrentTreatsAsync(currentTreatsUpdate);

        // Act
        var result = await _sut.GetByIdAsync(account.Id, character.Id);

        // Assert
        result.Should().NotBeNull();

        result.Should().BeEquivalentTo(character, options => options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    [SkipIfEnvironmentMissingTheory]
    [InlineData(null, null, "Bingus")]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoItemsAreFound(string? characterClass, int? level, string? name)
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = account.Id,
                                              Page = 1,
                                              PageSize = 5,
                                              Class = characterClass,
                                              Level = level,
                                              Name = name
                                          };

        // Act
        IEnumerable<Character> result = await _sut.GetAllAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [SkipIfEnvironmentMissingTheory]
    [MemberData(nameof(GetSingleSearchCharacterNameData))]
    public async Task GetAllAsync_ShouldReturnItems_WhenItemsAreFound(Account account, Character character, string characterName)
    {
        // Arrange
        await _accountRepository.CreateAsync(account);

        if (!await _sut.ExistsByIdAsync(character.Id))
        {
            await _sut.CreateAsync(character);
        }

        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = account.Id,
                                              Page = 1,
                                              PageSize = 5,
                                              Name = characterName
                                          };

        // Act
        IEnumerable<Character> result = await _sut.GetAllAsync(options);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [SkipIfEnvironmentMissingTheory]
    [InlineData(SortOrder.unordered)]
    [InlineData(SortOrder.ascending)]
    [InlineData(SortOrder.descending)]
    public async Task GetAllAsync_ShouldReturnSortedList_WhenItemsAreSorted(SortOrder sortOrder)
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        List<Character> characters = Enumerable.Range(5, 10).Select(_ => Fakes.GenerateCharacter(account)).ToList();

        await _accountRepository.CreateAsync(account);
        foreach (Character character in characters)
        {
            await _sut.CreateAsync(character);
        }

        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = account.Id,
                                              SortField = "name",
                                              SortOrder = sortOrder,
                                              Page = 1,
                                              PageSize = 25
                                          };

        // Act
        IEnumerable<Character> dbResult = await _sut.GetAllAsync(options);
        List<Character> results = dbResult.ToList();

        // Assert
        results.Should().NotBeEmpty();

        switch (sortOrder)
        {
            case SortOrder.ascending:
                results.Should().BeInAscendingOrder(x => x.Name, StringComparer.CurrentCulture);
                break;
            case SortOrder.descending:
                results.Should().BeInDescendingOrder(x => x.Name, StringComparer.CurrentCulture);
                break;
            case SortOrder.unordered:
            default:
                break;
        }
    }

    [Fact]
    public async Task GetCountAsync_ShouldReturnZero_WhenNoItemsAreCounted()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = account.Id,
                                              Page = 1,
                                              PageSize = 5,
                                              Name = "Bingus"
                                          };
        // Act
        int result = await _sut.GetCountAsync(options);

        // Assert
        result.Should().Be(0);
    }

    [SkipIfEnvironmentMissingFact]
    public async Task GetCountAsync_ShouldReturnOne_WhenItemsAreCounted()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = account.Id,
                                              Page = 1,
                                              PageSize = 5,
                                              Name = character.Name
                                          };
        // Act
        int result = await _sut.GetCountAsync(options);

        // Assert
        result.Should().Be(1);
    }

    [SkipIfEnvironmentMissingFact]
    public async Task ExistsByIdAsync_ShouldReturnFalse_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        // Act
        bool result = await _sut.ExistsByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [SkipIfEnvironmentMissingFact]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenIdIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        // Act
        bool result = await _sut.ExistsByIdAsync(character.Id);

        // Assert
        result.Should().BeTrue();
    }
    
    [SkipIfEnvironmentMissingFact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenCharacterIsNotDeleted()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        // Act
        bool result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenCharacterIsDeleted()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        await _accountRepository.CreateAsync(account);
        await _sut.CreateAsync(character);

        // Act
        bool result = await _sut.DeleteAsync(character.Id);

        // Assert
        result.Should().BeTrue();

        Character? deletedCharacter = await _sut.GetByIdAsync(account.Id, character.Id, true);

        deletedCharacter.Should().NotBeNull();
        deletedCharacter.DeletedUtc.Should().NotBeNull();
    }

    public static IEnumerable<object[]> GetSingleSearchCharacterNameData()
    {
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        yield return [account, character, character.Name];
        yield return [account, character, character.Name.ToLowerInvariant()];
        yield return [account, character, character.Name.ToUpperInvariant()];
        yield return [account, character, character.Name.RandomizeCasing()];
    }
}