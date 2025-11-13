using FluentAssertions;
using MagicalKitties.Application.Database;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Repositories.Implementation;
using MagicalKitties.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Integration;

public class HumanRepositoryTests : IClassFixture<ApplicationApiFactory>
{
    private readonly AccountRepository _accountRepository;
    private readonly CharacterUpdateRepository _characterUpdateRepository;
    private readonly CharacterRepository _characterRepository;
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly ProblemRepository _problemRepository;

    public HumanRepositoryTests(ApplicationApiFactory apiFactory)
    {
        IDbConnectionFactory dbConnectionFactory = apiFactory.Services.GetRequiredService<IDbConnectionFactory>();

        _sut = new HumanRepository(dbConnectionFactory, _dateTimeProvider);
        _accountRepository = new AccountRepository(dbConnectionFactory, _dateTimeProvider);
        _characterUpdateRepository = new CharacterUpdateRepository(dbConnectionFactory, _dateTimeProvider);
        _characterRepository = new CharacterRepository(dbConnectionFactory, _dateTimeProvider);
        _problemRepository = new ProblemRepository(dbConnectionFactory, _dateTimeProvider);
    }
    
    public HumanRepository _sut { get; set; }

    [SkipIfEnvironmentMissingFact]
    public async Task DeleteAsync_ShouldReturnTrueAndUpdateCharacter_WhenHumanIsDeletedAndHasNoProblems()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account).WithHumanData();

        Human human = character.Humans.First();

        _dateTimeProvider.GetUtcNow().Returns(DateTime.UtcNow);

        await _accountRepository.CreateAsync(account);
        await _characterRepository.CreateAsync(character);
        await _sut.CreateAsync(human);

        // Act
        bool result = await _sut.DeleteAsync(human.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrueAndUpdateCharacter_WhenHumanIsDeletedAndHasProblems()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account).WithHumanData();
        Human human = character.Humans.First();
        DateTime now = DateTime.UtcNow;

        _dateTimeProvider.GetUtcNow().Returns(now);

        await _accountRepository.CreateAsync(account);
        await _characterRepository.CreateAsync(character);
        await _sut.CreateAsync(human);
        
        foreach (Problem humanProblem in human.Problems)
        {
            await _problemRepository.CreateProblemAsync(humanProblem);
        }

        // Act
        bool result = await _sut.DeleteAsync(human.Id);

        // Assert
        result.Should().BeTrue();

        Character? updatedCharacter = await _characterRepository.GetByIdAsync(account.Id, character.Id);
        updatedCharacter.Should().NotBeNull();
        updatedCharacter.UpdatedUtc.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));

        Human? updatedHuman = await _sut.GetByIdAsync(character.Id, human.Id, true);
        updatedHuman.Should().NotBeNull();
        updatedHuman.DeletedUtc.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        foreach (Problem updatedHumanProblem in updatedHuman.Problems)
        {
            updatedHumanProblem.DeletedUtc.Should().NotBeNull();
            updatedHumanProblem.DeletedUtc.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        }
    }
    
}