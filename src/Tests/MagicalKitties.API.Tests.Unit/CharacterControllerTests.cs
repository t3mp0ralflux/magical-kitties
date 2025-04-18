using MagicalKitties.Api.Controllers;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Contracts.Requests.Characters;
using MagicalKitties.Contracts.Responses.Characters;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Testing.Common;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;

namespace MagicalKitties.API.Tests.Unit;

public class CharacterControllerTests
{
    private readonly IAccountService _accountService = Substitute.For<IAccountService>();
    private readonly ICharacterService _characterService = Substitute.For<ICharacterService>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    public CharacterController _sut;

    public CharacterControllerTests()
    {
        _sut = new CharacterController(_accountService, _characterService, _dateTimeProvider);
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenAccountNotFound()
    {
        // Arrange
        _accountService.GetByEmailAsync(Arg.Any<string>()).Returns((Account?)null);

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.Create(CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenCharacterIsCreated()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        CharacterResponse expectedResponse = new Character
                                             {
                                                 Id = Guid.NewGuid(),
                                                 AccountId = account.Id,
                                                 Username = account.Username,
                                                 Name = $"{account.Username}'s Unnamed Character",
                                                 Attributes = 
                                                 [
                                                     new Attribute()
                                                     {
                                                         Id = Guid.NewGuid(),
                                                         Name = "Cute",
                                                         Value = 0
                                                     },
                                                     new Attribute()
                                                     {
                                                         Id = Guid.NewGuid(),
                                                         Name = "Cunning",
                                                         Value = 0
                                                     },
                                                     new Attribute()
                                                     {
                                                         Id = Guid.NewGuid(),
                                                         Name = "Fierce",
                                                         Value = 0
                                                     },
                                                 ],
                                                 CurrentOwies = 0,
                                                 CurrentInjuries = 0,
                                                 CurrentTreats = 0,
                                                 MaxOwies = 2,
                                                 StartingTreats = 2
                                             }.ToResponse();

        // Act
        CreatedAtActionResult result = (CreatedAtActionResult)await _sut.Create(CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(201);
        result.ActionName.Should().Be(nameof(_sut.Get));
        result.Value.Should().BeEquivalentTo(expectedResponse, options => options
                                                                   .Excluding(x => x.Id)
                                                                   .For(x=>x.Attributes)
                                                                   .Exclude(y=>y.Id));
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns((Account?)null);

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.Get(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.Get(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Get_ShouldReturnCharacter_WhenCharacterIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        Character character = Fakes.GenerateCharacter(account);

        _characterService.GetByIdAsync(character.Id).Returns(character);

        CharacterResponse expectedResponse = character.ToResponse();

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.Get(character.Id, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetAll_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns((Account?)null);

        GetAllCharactersRequest request = new()
                                          {
                                              Page = 1,
                                              PageSize = 5
                                          };

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.GetAll(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoItemsAreFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        GetAllCharactersRequest request = new()
                                          {
                                              Name = "Test",
                                              Page = 1,
                                              PageSize = 5
                                          };

        CharactersResponse expectedResponse = new()
                                              {
                                                  Items = [],
                                                  Page = request.Page,
                                                  PageSize = request.PageSize,
                                                  Total = 0
                                              };
        // Act
        OkObjectResult result = (OkObjectResult)await _sut.GetAll(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCharacters_WhenItemsAreFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        Character character = Fakes.GenerateCharacter(account);

        GetAllCharactersRequest request = new()
                                          {
                                              Name = "Test",
                                              Page = 1,
                                              PageSize = 5
                                          };

        List<Character> characters = [character];

        _characterService.GetAllAsync(Arg.Any<GetAllCharactersOptions>()).Returns(characters);
        _characterService.GetCountAsync(Arg.Any<GetAllCharactersOptions>()).Returns(characters.Count);

        CharactersResponse expectedResponse = characters.ToGetAllResponse(request.Page, request.PageSize, characters.Count);

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.GetAll(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns((Account?)null);

        CharacterUpdateRequest request = new()
                                         {
                                             Id = Guid.NewGuid(),
                                             Name = string.Empty
                                         };

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.Update(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        _characterService.UpdateAsync(Arg.Any<Character>()).Returns(false);

        CharacterUpdateRequest request = new()
                                         {
                                             Id = Guid.NewGuid(),
                                             Name = string.Empty
                                         };

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.Update(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Update_ShouldReturnUpdatedCharacter_WhenCharacterIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        _characterService.UpdateAsync(Arg.Any<Character>()).Returns(true);

        CharacterUpdateRequest request = new()
                                         {
                                             Id = Guid.NewGuid(),
                                             Name = "Steve"
                                         };

        CharacterResponse expectedResponse = request.ToCharacter(account).ToResponse();

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.Update(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns((Account?)null);

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.Delete(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        _characterService.DeleteAsync(Guid.NewGuid()).Returns(false);

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.Delete(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenCharacterIsDeleted()
    {
        // Arrange
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(Arg.Any<string?>()).Returns(account);

        Guid characterId = Guid.NewGuid();

        _characterService.DeleteAsync(characterId).Returns(true);

        // Act
        NoContentResult result = (NoContentResult)await _sut.Delete(characterId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(204);
    }
}