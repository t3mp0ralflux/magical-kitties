using FluentAssertions;
using MagicalKitties.Api.Controllers;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.API.Tests.Unit;

public class HumanControllerTests
{
    private readonly IHumanService _humanService = Substitute.For<IHumanService>();
    private readonly IAccountService _accountService = Substitute.For<IAccountService>();
    private readonly ICharacterService _characterService = Substitute.For<ICharacterService>();
    
    public HumanController _sut;

    public HumanControllerTests()
    {
        _sut = new HumanController(_humanService, _accountService, _characterService);
    }

    [Fact]
    public async Task CreateProblem_ReturnsNewProblem_WhenProblemIsCreatedForCharacter()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account).WithHumanData();
        Human human = character.Humans.First();

        Problem newProblem = new Problem
                             {
                                 Id = Guid.Empty,
                                 HumanId = human.Id,
                                 Source = "",
                                 CustomSource = null,
                                 Emotion = "",
                                 CustomEmotion = null,
                                 Rank = 0,
                                 Solved = false,
                                 DeletedUtc = null
                             };

        _sut.ControllerContext = Utilities.CreateControllerContext(account.Email);
        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _humanService.CreateProblemAsync(character.Id, human.Id).Returns(newProblem);
        
        // Act
        CreatedResult result = (CreatedResult)await _sut.CreateProblem(character.Id, human.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeEquivalentTo(new Problem
                                             {
                                                 Id = Guid.Empty,
                                                 HumanId = human.Id,
                                                 Source = "",
                                                 CustomSource = null,
                                                 Emotion = "",
                                                 CustomEmotion = null,
                                                 Rank = 0,
                                                 Solved = false,
                                                 DeletedUtc = null
                                             }, options => options.Excluding(x=>x.DeletedUtc));
    }
}