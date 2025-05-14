using FluentAssertions;
using FluentValidation;
using MagicalKitties.Api.Controllers;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Account;
using MagicalKitties.Contracts.Responses.Account;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Testing.Common;
using MKCtr = MagicalKitties.Contracts.Models;

namespace MagicalKitties.API.Tests.Unit;

public class AccountControllerTests
{
    private readonly  IAccountService _accountService = Substitute.For<IAccountService>();

    public AccountControllerTests()
    {
        _sut = new AccountController(_accountService);
    }

    public AccountController _sut { get; set; }

    [Fact]
    public async Task Create_ShouldThrowException_WhenRequestIsMissingRequiredInformation()
    {
        // Arrange
        Account fakeAccount = Fakes.GenerateAccount();
        _accountService.CreateAsync(Arg.Any<Account>()).Throws(new ValidationException("Information is required"));

        AccountCreateRequest request = new()
                                       {
                                           Email = fakeAccount.Email,
                                           FirstName = fakeAccount.FirstName,
                                           LastName = fakeAccount.LastName,
                                           Password = fakeAccount.Password,
                                           UserName = fakeAccount.Username
                                       };
        // Act
        Func<Task<BadRequestResult>> result = async () => (BadRequestResult)await _sut.Create(request, CancellationToken.None);

        // Assert
        await result.Should().ThrowAsync<ValidationException>("Information is required");
    }

    [Fact]
    public async Task Create_ShouldCreateAccount_WhenRequestInformationIsPresent()
    {
        // Arrange
        _accountService.CreateAsync(Arg.Any<Account>()).Returns(true);
        Account fakeAccount = Fakes.GenerateAccount();

        AccountCreateRequest request = new()
                                       {
                                           Email = fakeAccount.Email,
                                           FirstName = fakeAccount.FirstName,
                                           LastName = fakeAccount.LastName,
                                           Password = fakeAccount.Password,
                                           UserName = fakeAccount.Username
                                       };

        AccountResponse expectedResponse = request.ToAccount().ToResponse();

        // Act
        CreatedAtActionResult result = (CreatedAtActionResult)await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
        result.ActionName.Should().Be(nameof(_sut.Get));
        result.RouteValues.Should().NotBeEmpty();
        result.Value.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(y => y.Id));
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenAccountIsNotFound()
    {
        // Arrange
        _accountService.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Account?)null);

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.Get(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Get_ShouldReturnAccount_WhenAccountIsFound()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();

        Account account = Fakes.GenerateAccount();

        _accountService.GetByIdAsync(accountId, Arg.Any<CancellationToken>()).Returns(account);

        AccountResponse expectedResponse = account.ToResponse();
        // Act
        OkObjectResult result = (OkObjectResult)await _sut.Get(accountId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetAll_ShouldThrowException_WhenWrongSortFieldsArePassedIn()
    {
        // Arrange
        GetAllAccountsRequest request = new()
                                        {
                                            Page = 1,
                                            PageSize = 5
                                        };

        _accountService.GetAllAsync(Arg.Any<GetAllAccountsOptions>(), CancellationToken.None).Throws(new ValidationException("Bad Search Term"));
        // Act
        Func<Task<IActionResult>> action = async () => await _sut.GetAll(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Bad Search Term");
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyResult_WhenNoItemsAreFound()
    {
        // Arrange
        GetAllAccountsRequest request = new()
                                        {
                                            UserName = "Test",
                                            Page = 1,
                                            PageSize = 5
                                        };

        AccountsResponse expectedResponse = new()
                                            {
                                                Items = [],
                                                Page = 1,
                                                PageSize = 5,
                                                Total = 0
                                            };

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.GetAll(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetAll_ShouldReturnResults_WhenItemsAreFound()
    {
        // Arrange
        GetAllAccountsRequest request = new()
                                        {
                                            UserName = "Test",
                                            Page = 1,
                                            PageSize = 5
                                        };

        GetAllAccountsOptions requestOptions = request.ToOptions();

        Random random = new();

        List<Account> accounts = Enumerable.Range(5, random.Next(1, 15)).Select(x => Fakes.GenerateAccount()).ToList();

        _accountService.GetAllAsync(Arg.Any<GetAllAccountsOptions>(), CancellationToken.None).Returns(accounts);
        _accountService.GetCountAsync(requestOptions.UserName, CancellationToken.None).Returns(accounts.Count);

        AccountsResponse expectedResponse = accounts.ToResponse(request.Page, request.PageSize, accounts.Count());

        // Act
        OkObjectResult results = (OkObjectResult)await _sut.GetAll(request, CancellationToken.None);

        // Assert
        results.Should().NotBeNull();
        results.StatusCode.Should().Be(200);
        results.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenAccountIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        AccountUpdateRequest request = new()
                                       {
                                           FirstName = "New First",
                                           LastName = "New Last",
                                           Username = "",
                                           Password = "",
                                           Email = "",
                                           AccountStatus = (MKCtr.AccountStatus)account.AccountStatus,
                                           AccountRole = (MKCtr.AccountRole)account.AccountRole
                                       };

        _accountService.UpdateAsync(Arg.Any<Account>(), CancellationToken.None).Returns((Account?)null);

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.Update(account.Id, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenAccountIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        AccountUpdateRequest request = new()
                                       {
                                           FirstName = "New First",
                                           LastName = "New Last",
                                           Username = string.Empty,
                                           Password = string.Empty,
                                           Email = string.Empty,
                                           AccountStatus = (MKCtr.AccountStatus)account.AccountStatus,
                                           AccountRole = (MKCtr.AccountRole)account.AccountRole
                                       };

        AccountResponse expectedOutput = request.ToAccount(account.Id).ToResponse();

        _accountService.UpdateAsync(Arg.Any<Account>(), CancellationToken.None).Returns(account);

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.Update(account.Id, request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedOutput, options => options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenIdIsNotFound()
    {
        // Arrange
        Guid missingId = Guid.NewGuid();

        _accountService.DeleteAsync(missingId, CancellationToken.None).Returns(false);

        // Act
        NotFoundResult? result = (NotFoundResult)await _sut.Delete(missingId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenIdIsFound()
    {
        // Arrange
        Guid missingId = Guid.NewGuid();

        _accountService.DeleteAsync(missingId, CancellationToken.None).Returns(true);

        // Act
        NoContentResult? result = (NoContentResult)await _sut.Delete(missingId, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Activate_ShouldThrowException_WhenAccountIsNotFound()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ActivateAsync(Arg.Any<AccountActivation>()).Throws(new ValidationException("No account found"));

        // Act
        Func<Task<IActionResult>> action = async () => await _sut.Activate(username, activationcode, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("No account found");
    }

    [Fact]
    public async Task Activate_ShouldThrowException_WhenActivationIsNotValid()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ActivateAsync(Arg.Any<AccountActivation>()).Throws(new ValidationException("Activation Code has expired"));

        // Act
        Func<Task<IActionResult>> action = async () => await _sut.Activate(username, activationcode, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Activation Code has expired");
    }

    [Fact]
    public async Task Activate_ShouldThrowException_WhenActivationFails()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ActivateAsync(Arg.Any<AccountActivation>()).Throws(new ValidationException("Couldn't activate"));

        // Act
        Func<Task<IActionResult>> action = async () => await _sut.Activate(username, activationcode, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Couldn't activate");
    }

    [Fact]
    public async Task Activate_ShouldThrowException_WhenActivationFailsInDb()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ActivateAsync(Arg.Any<AccountActivation>()).Throws(new ValidationException("Error, yo"));

        // Act
        Func<Task<IActionResult>>? action = async () => await _sut.Activate(username, activationcode, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>("Error, yo");
    }

    [Fact]
    public async Task Activate_ShouldReturnOk_WhenActivationPasses()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ActivateAsync(Arg.Any<AccountActivation>()).Returns(true);

        AccountActivationResponse expectedResponse = new()
                                                     {
                                                         Username = username
                                                     };

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.Activate(username, activationcode, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResendActivation_ShouldThrowException_WhenAccountIsNotFound()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ResendActivationAsync(Arg.Any<AccountActivation>()).Throws(new ValidationException("Account not found"));

        // Act
        Func<Task<IActionResult>> action = async () => await _sut.ResendActivation(username, activationcode, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Account not found");
    }

    [Fact]
    public async Task ResendActivation_ShouldThrowException_WhenActivationIsNotValid()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ResendActivationAsync(Arg.Any<AccountActivation>()).Throws(new ValidationException("Activation invalid"));

        // Act
        Func<Task<IActionResult>> action = async () => await _sut.ResendActivation(username, activationcode, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ValidationException>("Activation invalid");
    }

    [Fact]
    public async Task Activate_ShouldThrowException_WhenResendActivationFailsInDb()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ResendActivationAsync(Arg.Any<AccountActivation>()).Throws(new ValidationException("Error, yo"));

        // Act
        Func<Task<IActionResult>>? action = async () => await _sut.ResendActivation(username, activationcode, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>("Error, yo");
    }

    [Fact]
    public async Task ResendActivation_ShouldReturnOk_WhenActivationIsResent()
    {
        // Arrange
        string username = "TestUsername";
        string activationcode = "Activate";

        _accountService.ResendActivationAsync(Arg.Any<AccountActivation>()).Returns(true);

        AccountActivationResponse expectedResponse = new()
                                                     {
                                                         Username = username
                                                     };

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.ResendActivation(username, activationcode, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }
}