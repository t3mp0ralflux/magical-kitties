using System.Security.Claims;
using FluentAssertions;
using MagicalKitties.Api.Controllers;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Testing.Common;
using Xunit.Sdk;
using MKCtrApplicationCharacterUpdates = MagicalKitties.Application.Models.Characters.Updates;
using MKCtrCharacterRequests = MagicalKitties.Contracts.Requests.Characters;


namespace MagicalKitties.API.Tests.Unit;

public class CharacterUpdateControllerTests
{
    private readonly IAccountService _accountService = Substitute.For<IAccountService>();
    private readonly ICharacterService _characterService = Substitute.For<ICharacterService>();
    private readonly ICharacterUpdateService _characterUpdateService = Substitute.For<ICharacterUpdateService>();
    private readonly ICharacterUpgradeService _characterUpgradeService = Substitute.For<ICharacterUpgradeService>();
    
    public CharacterUpdateController _sut;

    public CharacterUpdateControllerTests()
    {
        _sut = new CharacterUpdateController(_accountService, _characterService, _characterUpdateService, _characterUpgradeService);
    }

    [Theory]
    [MemberData(nameof(GetDescriptionOptions))]
    public async Task UpdateDescription_ShouldReturnUnauthorized_WhenAccountIsNull(MKCtrCharacterRequests.DescriptionOption option)
    {
        // Arrange
        MKCtrCharacterRequests.CharacterDescriptionUpdateRequest request = new()
                                                                           {
                                                                               CharacterId = Guid.NewGuid()
                                                                           };

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.UpdateDescription(option, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task UpdateDescription_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = CreateControllerContext(account.Email);
        
        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpdateService.UpdateDescriptionAsync(Arg.Any<MKCtrApplicationCharacterUpdates.DescriptionOption>(), Arg.Any<MKCtrApplicationCharacterUpdates.DescriptionUpdate>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        MKCtrCharacterRequests.CharacterDescriptionUpdateRequest request = new()
                                                                           {
                                                                               CharacterId = character.Id
                                                                           };

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.UpdateDescription(MKCtrCharacterRequests.DescriptionOption.name, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateDescription_ShouldReturnUnauthorized_WhenCharacterIsFoundButNotOwnedByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpdateService.UpdateDescriptionAsync(Arg.Any<MKCtrApplicationCharacterUpdates.DescriptionOption>(), Arg.Any<MKCtrApplicationCharacterUpdates.DescriptionUpdate>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        MKCtrCharacterRequests.CharacterDescriptionUpdateRequest request = new()
                                                                           {
                                                                               CharacterId = character.Id
                                                                           };

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.UpdateDescription(MKCtrCharacterRequests.DescriptionOption.name, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }
    
    [Theory]
    [MemberData(nameof(GetAttributeOptions))]
    public async Task UpdateAttribute_ShouldReturnUnauthorized_WhenAccountIsNull(MKCtrCharacterRequests.AttributeOption option)
    {
        // Arrange
        MKCtrCharacterRequests.CharacterAttributeUpdateRequest request = new()
                                                                           {
                                                                               CharacterId = Guid.NewGuid()
                                                                           };

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.UpdateAttribute(option, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }
    
    [Fact]
    public async Task UpdateAttribute_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpdateService.UpdateAttributeAsync(Arg.Any<MKCtrApplicationCharacterUpdates.AttributeOption>(), Arg.Any<MKCtrApplicationCharacterUpdates.AttributeUpdate>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        MKCtrCharacterRequests.CharacterAttributeUpdateRequest request = new()
                                                                           {
                                                                               CharacterId = character.Id
                                                                           };

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.UpdateAttribute(MKCtrCharacterRequests.AttributeOption.cunning, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public async Task UpdateAttribute_ShouldReturnUnauthorized_WhenCharacterIsFoundButNotOwnedByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpdateService.UpdateAttributeAsync(Arg.Any<MKCtrApplicationCharacterUpdates.AttributeOption>(), Arg.Any<MKCtrApplicationCharacterUpdates.AttributeUpdate>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        MKCtrCharacterRequests.CharacterAttributeUpdateRequest request = new()
                                                                         {
                                                                             CharacterId = character.Id
                                                                         };

        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.UpdateAttribute(MKCtrCharacterRequests.AttributeOption.cunning, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task Reset_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        _characterService.GetByIdAsync(Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the GetByIdAsync function."));
        _characterUpdateService.Reset(Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));
        
        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.Reset(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task Reset_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = CreateControllerContext(account.Email);
        
        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpdateService.Reset(Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));
        
        // Act
        NotFoundResult result = (NotFoundResult)await _sut.Reset(character.Id, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Reset_ShouldReturnUnauthorized_WhenCharacterIsFoundButNotOwnedByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = CreateControllerContext(unauthorizedAccount.Email);
        
        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpdateService.Reset(Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));
        
        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.Reset(character.Id, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Theory]
    [MemberData(nameof(GetAttributeOptions))]
    public async Task UpsertUpgrade_ShouldReturnUnauthorized_WhenAccountIsNotFound(MKCtrCharacterRequests.AttributeOption option)
    {
        // Arrange
        MKCtrCharacterRequests.UpgradeUpsertRequest request = new MKCtrCharacterRequests.UpgradeUpsertRequest()
                                                              {
                                                                  AttributeOption = option,
                                                                  Block = 1,
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This doesn't matter"
                                                              };
        
        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.UpsertUpgrade(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        
        MKCtrCharacterRequests.UpgradeUpsertRequest request = new MKCtrCharacterRequests.UpgradeUpsertRequest()
                                                              {
                                                                  AttributeOption = MKCtrCharacterRequests.AttributeOption.cute,
                                                                  Block = 1,
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This doesn't matter"
                                                              };

        _sut.ControllerContext = CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpgradeService.UpsertUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));
        
        // Act
        NotFoundResult result = (NotFoundResult)await _sut.UpsertUpgrade(character.Id, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public async Task UpsertUpgrade_ShouldReturnUnauthorized_WhenCharacterIsFoundButNotOwnedByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        
        MKCtrCharacterRequests.UpgradeUpsertRequest request = new MKCtrCharacterRequests.UpgradeUpsertRequest()
                                                              {
                                                                  AttributeOption = MKCtrCharacterRequests.AttributeOption.cute,
                                                                  Block = 1,
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This doesn't matter"
                                                              };

        _sut.ControllerContext = CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpgradeService.UpsertUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));
        
        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.UpsertUpgrade(character.Id, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task RemoveUpgrade_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        MKCtrCharacterRequests.UpgradeRemoveRequest request = new MKCtrCharacterRequests.UpgradeRemoveRequest()
                                                              {
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This isn't relevant"
                                                              };
        
        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.RemoveUpgrade(Guid.NewGuid(), request, CancellationToken.None);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }
    
    [Fact]
    public async Task RemoveUpgrade_ShouldReturnNotFound_WhenCharacterIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        
        MKCtrCharacterRequests.UpgradeRemoveRequest request = new MKCtrCharacterRequests.UpgradeRemoveRequest()
                                                              {
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This isn't relevant"
                                                              };
        
        _sut.ControllerContext = CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpgradeService.RemoveUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));
        
        // Act
        NotFoundResult result = (NotFoundResult)await _sut.RemoveUpgrade(character.Id, request, CancellationToken.None);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public async Task RemoveUpgrade_ShouldReturnUnauthorized_WhenCharacterIsFoundButNotOwnedByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        
        MKCtrCharacterRequests.UpgradeRemoveRequest request = new MKCtrCharacterRequests.UpgradeRemoveRequest()
                                                              {
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This isn't relevant"
                                                              };
        
        _sut.ControllerContext = CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpgradeService.RemoveUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));
        
        // Act
        UnauthorizedResult result = (UnauthorizedResult)await _sut.RemoveUpgrade(character.Id, request, CancellationToken.None);
        
        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    public static IEnumerable<object[]> GetDescriptionOptions()
    {
        MKCtrCharacterRequests.DescriptionOption[] values = Enum.GetValues<MKCtrCharacterRequests.DescriptionOption>();

        foreach (MKCtrCharacterRequests.DescriptionOption descriptionOption in values)
        {
            yield return [descriptionOption];
        }
    }
    
    public static IEnumerable<object[]> GetAttributeOptions()
    {
        MKCtrCharacterRequests.AttributeOption[] values = Enum.GetValues<MKCtrCharacterRequests.AttributeOption>();

        foreach (MKCtrCharacterRequests.AttributeOption descriptionOption in values)
        {
            yield return [descriptionOption];
        }
    }

    public static ControllerContext CreateControllerContext(string email)
    {
        ControllerContext result = new ControllerContext
                                   {
                                       HttpContext = new DefaultHttpContext
                                                     {
                                                         User = new ClaimsPrincipal()
                                                     }
                                   };

        result.HttpContext.User.AddIdentity(new ClaimsIdentity([new Claim(ClaimTypes.Email, email)]));

        return result;
    }
}