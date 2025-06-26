using FluentAssertions;
using MagicalKitties.Api.Controllers;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Api.Services;
using MagicalKitties.Application;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Auth;
using MagicalKitties.Contracts.Responses.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.API.Tests.Unit;

public class AuthControllerTests
{
    private readonly IAccountService _accountService = Substitute.For<IAccountService>();
    private readonly IRefreshTokenService _refreshTokenService = Substitute.For<IRefreshTokenService>();
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IJwtTokenService _jwtService = Substitute.For<IJwtTokenService>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    
    public AuthControllerTests()
    {
        _sut = new AuthController(_accountService, _refreshTokenService, _passwordHasher, _jwtService, _authService);
    }

    public AuthController _sut { get; set; }

    [Fact]
    public async Task Login_ShouldReturnNotFound_WhenEmailIsNotFound()
    {
        // Arrange
        LoginRequest request = new()
                               {
                                   Email = "test@test.test",
                                   Password = "Bingus"
                               };

        _accountService.GetByEmailAsync(request.Email, CancellationToken.None).Returns((Account?)null);

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.LoginByPassword(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task LoginByPassword_ShouldReturnNotFound_WhenUsernameIsNotFound()
    {
        // Arrange
        LoginRequest request = new()
                               {
                                   Email = "Dingus",
                                   Password = "Bingus"
                               };

        _accountService.GetByUsernameAsync(request.Email, CancellationToken.None).Returns((Account?)null);
        // Act
        NotFoundResult result = (NotFoundResult)await _sut.LoginByPassword(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task LoginByPassword_ShouldReturnUnauthorized_WhenEmailIsCorrectAndPasswordIsIncorrect()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        LoginRequest request = new()
                               {
                                   Email = account.Email,
                                   Password = account.Password
                               };

        _accountService.GetByEmailAsync(request.Email, CancellationToken.None).Returns(account);

        _passwordHasher.Verify(request.Password, account.Password).Returns(false);

        // Act
        UnauthorizedObjectResult result = (UnauthorizedObjectResult)await _sut.LoginByPassword(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().Be("Username or password is incorrect");
    }

    [Fact]
    public async Task LoginByPassword_ShouldReturnUnauthorized_WhenUsernameIsCorrectAndPasswordIsIncorrect()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        LoginRequest request = new()
                               {
                                   Email = account.Username,
                                   Password = account.Password
                               };

        _accountService.GetByUsernameAsync(request.Email, CancellationToken.None).Returns(account);

        _passwordHasher.Verify(request.Password, account.Password).Returns(false);

        // Act
        UnauthorizedObjectResult result = (UnauthorizedObjectResult)await _sut.LoginByPassword(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().Be("Username or password is incorrect");
    }

    [Fact]
    public async Task LoginByPassword_ShouldReturnUnauthorized_WhenAccountIsNotActive()
    {
        // Arrange
        Account account = Fakes.GenerateAccount(AccountStatus.created);
        LoginRequest request = new()
                               {
                                   Email = account.Email,
                                   Password = account.Password
                               };

        _accountService.GetByEmailAsync(request.Email, CancellationToken.None).Returns(account);

        // Act
        UnauthorizedObjectResult result = (UnauthorizedObjectResult)await _sut.LoginByPassword(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().Be("You must activate your account before you can login");
    }

    [Fact]
    public async Task LoginByPassword_ShouldReturnJwtToken_WhenDataIsCorrect()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        LoginRequest request = new()
                               {
                                   Email = account.Email,
                                   Password = account.Password
                               };

        const string expectedToken = "ThisIsTheSingleBestTokenYouHaveEverSeen";
        const string expectedRefreshToken = "ThisIsARefreshingToken";

        _accountService.GetByEmailAsync(request.Email, CancellationToken.None).Returns(account);
        _passwordHasher.Verify(request.Password, account.Password).Returns(true);
        _jwtService.GenerateToken(account).Returns(expectedToken);
        _jwtService.GenerateRefreshToken().Returns(expectedRefreshToken);
        _authService.LoginAsync(Arg.Any<AccountLogin>()).Returns(true);

        LoginResponse expectedResult = new LoginResponse
                                       {
                                           Account = account.ToResponse(),
                                           AccessToken = expectedToken,
                                           RefreshToken = expectedRefreshToken
                                       };

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.LoginByPassword(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task LoginByToken_ShouldReturnFalse_WhenTokenIsInvalid()
    {
        // Arrange
        TokenRequest request = new TokenRequest
                               {
                                   AccessToken = "Bogus",
                                   RefreshToken = "Token"
                               };
        
        // Act
        UnauthorizedObjectResult result = (UnauthorizedObjectResult)await _sut.LoginByToken(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().Be("Token is invalid");
    }

    [Fact]
    public async Task LoginByToken_ShouldReturnNotFound_WhenAccountIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        
        const string token = "ThisIsALoginToken";
        const string refreshToken = "ThisIsARefreshToken";
        
        _jwtService.ValidateCustomToken(token).Returns(true);
        _jwtService.GetEmailFromToken(token).Returns(account.Email);
        
        TokenRequest request = new TokenRequest
                               {
                                   AccessToken = token,
                                   RefreshToken = refreshToken
                               };
        
        // Act
        NotFoundResult result = (NotFoundResult)await _sut.LoginByToken(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task LoginByToken_ShouldReturnUnauthorized_WhenAccountIsNotActive()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        account.AccountStatus = AccountStatus.banned;

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        
        const string token = "ThisIsALoginToken";
        const string refreshToken = "ThisIsARefreshToken";
        
        _jwtService.ValidateCustomToken(token).Returns(true);
        _jwtService.GetEmailFromToken(token).Returns(account.Email);
        
        TokenRequest request = new TokenRequest
                               {
                                   AccessToken = token,
                                   RefreshToken = refreshToken
                               };
        
        // Act
        UnauthorizedObjectResult result = (UnauthorizedObjectResult)await _sut.LoginByToken(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().Be("Your account status is not active. Contact support.");
    }
    
    [Fact]
    public async Task LoginByToken_ShouldReturnUnauthorized_WhenNoRefreshTokenExists()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        _refreshTokenService.Exists(account.Id).Returns(false);
        
        const string token = "ThisIsALoginToken";
        const string refreshToken = "ThisIsARefreshToken";
        
        _jwtService.ValidateCustomToken(token).Returns(true);
        _jwtService.GetEmailFromToken(token).Returns(account.Email);
        
        TokenRequest request = new TokenRequest
                               {
                                   AccessToken = token,
                                   RefreshToken = refreshToken
                               };
        
        // Act
        UnauthorizedObjectResult result = (UnauthorizedObjectResult)await _sut.LoginByToken(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().Be("Refresh token is invalid");
    }
    
    [Fact]
    public async Task LoginByToken_ShouldReturnUnauthorized_WhenRefreshTokenIsValidAndPayloadIsNot()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        
        const string token = "ThisIsALoginToken";
        const string refreshToken = "ThisIsARefreshToken";
        
        _jwtService.ValidateCustomToken(token).Returns(true);
        _jwtService.GetEmailFromToken(token).Returns(account.Email);
        
        RefreshToken storedToken = new RefreshToken
                                   {
                                       Id = Guid.NewGuid(),
                                       AccountId = account.Id,
                                       AccessToken = token,
                                       Token = refreshToken,
                                       ExpirationUtc = DateTime.UtcNow.AddMinutes(5)
                                   };
        
        _refreshTokenService.Exists(account.Id).Returns(true);
        _refreshTokenService.GetRefreshToken(account.Id).Returns(storedToken);
        _refreshTokenService.ValidateRefreshToken(account.Id, Arg.Any<AuthToken>()).Returns(false);

        _jwtService.ValidateCustomToken(Arg.Any<string>()).Returns(true);
        _jwtService.GetEmailFromToken(token).Returns(account.Email);
        
        TokenRequest request = new TokenRequest
                               {
                                   AccessToken = token,
                                   RefreshToken = "ThisIsAFakeRefreshToken"
                               };
        
        // Act
        UnauthorizedObjectResult result = (UnauthorizedObjectResult)await _sut.LoginByToken(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().Be("Refresh token is invalid");
    }
    
    [Fact]
    public async Task LoginByToken_ShouldReturnOk_WhenRefreshTokenAndPayloadAreValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountService.GetByEmailAsync(account.Email).Returns(account);
        
        const string token = "ThisIsALoginToken";
        const string refreshToken = "ThisIsARefreshToken";

        RefreshToken storedToken = new RefreshToken
                                   {
                                       Id = Guid.NewGuid(),
                                       AccountId = account.Id,
                                       AccessToken = token,
                                       Token = refreshToken,
                                       ExpirationUtc = DateTime.UtcNow.AddMinutes(5)
                                   };
        
        _jwtService.ValidateCustomToken(token).Returns(true);
        _jwtService.GetEmailFromToken(token).Returns(account.Email);
        
        _refreshTokenService.Exists(account.Id).Returns(true);
        _refreshTokenService.GetRefreshToken(account.Id).Returns(storedToken);
        _refreshTokenService.ValidateRefreshToken(account.Id, Arg.Any<AuthToken>()).Returns(true);

        const string expectedAccessToken = "ThisIsANewToken";
        const string expectedRefreshToken = "ThisIsANewRefreshToken";

        _jwtService.GenerateToken(Arg.Any<Account>()).Returns(expectedAccessToken);
        _jwtService.GenerateRefreshToken().Returns(expectedRefreshToken);

        _refreshTokenService.UpsertRefreshToken(Arg.Any<Account>(), expectedAccessToken, expectedRefreshToken).Returns(new RefreshToken
                                                                                                                       {
                                                                                                                           Id = Guid.NewGuid(),
                                                                                                                           AccountId = account.Id,
                                                                                                                           AccessToken = expectedAccessToken,
                                                                                                                           Token = expectedRefreshToken,
                                                                                                                           ExpirationUtc = DateTime.UtcNow.AddMinutes(5)
                                                                                                                       });
        
        TokenRequest request = new TokenRequest
                               {
                                   AccessToken = token,
                                   RefreshToken = refreshToken
                               };

        LoginResponse expectedResponse = new LoginResponse
                                         {
                                             Account = account.ToResponse(),
                                             AccessToken = expectedAccessToken,
                                             RefreshToken = expectedRefreshToken
                                         };
        
        // Act
        OkObjectResult result = (OkObjectResult)await _sut.LoginByToken(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task RequestPasswordReset_ShouldReturnOk_WhenEmailIsNotFound()
    {
        // Arrange
        const string email = "email@email.com";
        _accountService.RequestPasswordReset(email).Returns(false);

        PasswordResetRequest request = new PasswordResetRequest { Email = email, Password = "test", ResetCode = "test" };

        PasswordResetResponse expectedResponse = new PasswordResetResponse { Email = email };

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.RequestPasswordReset(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task RequestPasswordReset_ShouldReturnOk_WhenEmailIsFound()
    {
        // Arrange
        const string email = "email@email.com";
        _accountService.RequestPasswordReset(email).Returns(true);
        
        PasswordResetRequest request = new PasswordResetRequest { Email = email, Password = "test", ResetCode = "test" };
        
        PasswordResetResponse expectedResponse = new PasswordResetResponse { Email = email };

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.RequestPasswordReset(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task PasswordReset_ShouldReturnNotFound_WhenAccountIsNotFound()
    {
        // Arrange
        PasswordResetRequest request = new()
                                       {
                                           Email = "email@email.com",
                                           Password = "thisisanewpassword",
                                           ResetCode = "069420"
                                       };

        _accountService.ExistsByEmailAsync(request.Email).Returns(false);

        // Act
        NotFoundResult result = (NotFoundResult)await _sut.PasswordReset(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task PasswordReset_ShouldReturnOk_WhenPasswordIsReset()
    {
        // Arrange
        PasswordResetRequest request = new()
                                       {
                                           Email = "email@email.com",
                                           Password = "thisisanewpassword",
                                           ResetCode = "069420"
                                       };

        _accountService.ExistsByEmailAsync(request.Email).Returns(true);
        _accountService.ResetPasswordAsync(Arg.Any<PasswordReset>()).Returns(true);

        PasswordResetResponse expectedResponse = request.ToReset().ToResponse();

        // Act
        OkObjectResult result = (OkObjectResult)await _sut.PasswordReset(request, CancellationToken.None);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }
}