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

        _sut.ControllerContext = Utilities.CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns(false);
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

        _sut.ControllerContext = Utilities.CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns((bool?)null);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpdateService.UpdateDescriptionAsync(Arg.Any<MKCtrApplicationCharacterUpdates.DescriptionOption>(), Arg.Any<MKCtrApplicationCharacterUpdates.DescriptionUpdate>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        MKCtrCharacterRequests.CharacterDescriptionUpdateRequest request = new()
                                                                           {
                                                                               CharacterId = character.Id
                                                                           };

        // Act
        ForbidResult result = (ForbidResult)await _sut.UpdateDescription(MKCtrCharacterRequests.DescriptionOption.name, request, CancellationToken.None);

        // Assert
        result.Should().NotBe(null);
    }

    [Theory]
    [MemberData(nameof(GetUpgradeOptions))]
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

        _sut.ControllerContext = Utilities.CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns(false);
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
    public async Task UpdateAttribute_ShouldReturnForbidden_WhenCharacterIsFoundButNotOwnedByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = Utilities.CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns((bool?)null);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpdateService.UpdateAttributeAsync(Arg.Any<MKCtrApplicationCharacterUpdates.AttributeOption>(), Arg.Any<MKCtrApplicationCharacterUpdates.AttributeUpdate>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        MKCtrCharacterRequests.CharacterAttributeUpdateRequest request = new()
                                                                         {
                                                                             CharacterId = character.Id
                                                                         };

        // Act
        ForbidResult result = (ForbidResult)await _sut.UpdateAttribute(MKCtrCharacterRequests.AttributeOption.cunning, request, CancellationToken.None);

        // Assert
        result.Should().NotBe(null);
    }

    [Fact]
    public async Task Reset_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        _characterService.GetByIdAsync(Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the GetByIdAsync function."));
        _characterUpdateService.Reset(account.Id, Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

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

        _sut.ControllerContext = Utilities.CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns(false);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpdateService.Reset(account.Id, Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.Reset(character.Id, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Reset_ShouldReturnForbidden_WhenCharacterIsFoundByUnauthorizedAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        _sut.ControllerContext = Utilities.CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns((bool?)null);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpdateService.Reset(account.Id, Arg.Any<Guid>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Update function."));

        // Act
        ForbidResult result = (ForbidResult)await _sut.Reset(character.Id, CancellationToken.None);

        // Assert
        result.Should().NotBe(null);
    }

    [Theory]
    [MemberData(nameof(GetUpgradeOptions))]
    public async Task UpsertUpgrade_ShouldReturnUnauthorized_WhenAccountIsNotFound(MKCtrCharacterRequests.UpgradeOption option)
    {
        // Arrange
        MKCtrCharacterRequests.UpgradeUpsertRequest request = new()
                                                              {
                                                                  Block = 1,
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = option,
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

        MKCtrCharacterRequests.UpgradeUpsertRequest request = new()
                                                              {
                                                                  Block = 1,
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This doesn't matter"
                                                              };

        _sut.ControllerContext = Utilities.CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns(false);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpgradeService.UpsertUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.UpsertUpgrade(character.Id, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpsertUpgrade_ShouldReturnForbidden_WhenCharacterIsNotFoundByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        MKCtrCharacterRequests.UpgradeUpsertRequest request = new()
                                                              {
                                                                  Block = 1,
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This doesn't matter"
                                                              };

        _sut.ControllerContext = Utilities.CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns((bool?)null);
        _characterService.GetByIdAsync(character.Id).Returns(character);
        _characterUpgradeService.UpsertUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));

        // Act
        ForbidResult result = (ForbidResult)await _sut.UpsertUpgrade(character.Id, request, CancellationToken.None);

        // Assert
        result.Should().NotBe(null);
    }

    [Fact]
    public async Task RemoveUpgrade_ShouldReturnUnauthorized_WhenAccountIsNotFound()
    {
        // Arrange
        MKCtrCharacterRequests.UpgradeRemoveRequest request = new()
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

        MKCtrCharacterRequests.UpgradeRemoveRequest request = new()
                                                              {
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This isn't relevant"
                                                              };

        _sut.ControllerContext = Utilities.CreateControllerContext(account.Email);

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _characterService.ExistsByIdAsync(account.Id, character.Id).Returns(false);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpgradeService.RemoveUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.RemoveUpgrade(character.Id, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task RemoveUpgrade_ShouldReturnNotFound_WhenCharacterIsNotFoundByAccessingAccount()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Account unauthorizedAccount = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);

        MKCtrCharacterRequests.UpgradeRemoveRequest request = new()
                                                              {
                                                                  UpgradeId = Guid.NewGuid(),
                                                                  UpgradeOption = MKCtrCharacterRequests.UpgradeOption.attribute3,
                                                                  Value = "This isn't relevant"
                                                              };

        _sut.ControllerContext = Utilities.CreateControllerContext(unauthorizedAccount.Email);

        _accountService.GetByEmailAsync(unauthorizedAccount.Email).Returns(unauthorizedAccount);
        _characterService.GetByIdAsync(character.Id).Returns((Character?)null);
        _characterUpgradeService.RemoveUpgradeAsync(Arg.Any<UpgradeRequest>(), Arg.Any<CancellationToken>()).ThrowsAsync(FailException.ForFailure("Test should not have reached the Upsert function."));

        // Act
        ForbidResult result = (ForbidResult)await _sut.RemoveUpgrade(character.Id, request, CancellationToken.None);

        // Assert
        result.Should().NotBe(null);
    }

    public static IEnumerable<object[]> GetDescriptionOptions()
    {
        MKCtrCharacterRequests.DescriptionOption[] values = Enum.GetValues<MKCtrCharacterRequests.DescriptionOption>();

        foreach (MKCtrCharacterRequests.DescriptionOption descriptionOption in values)
        {
            yield return [descriptionOption];
        }
    }

    public static IEnumerable<object[]> GetUpgradeOptions()
    {
        MKCtrCharacterRequests.UpgradeOption[] values = Enum.GetValues<MKCtrCharacterRequests.UpgradeOption>();

        foreach (MKCtrCharacterRequests.UpgradeOption descriptionOption in values)
        {
            yield return [descriptionOption];
        }
    }
}