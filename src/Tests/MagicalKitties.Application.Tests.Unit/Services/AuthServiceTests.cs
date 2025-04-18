using FluentAssertions;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using NSubstitute;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();

    public AuthServiceTests()
    {
        _sut = new AuthService(_accountRepository);
    }

    public IAuthService _sut { get; set; }

    [Fact]
    public async Task Login_ShouldReturnFalse_WhenLoginFails()
    {
        // Arrange
        AccountLogin accountLogin = new()
                                    {
                                        Email = "test@test.com"
                                    };

        _accountRepository.LoginAsync(Arg.Any<AccountLogin>()).Returns(false);

        // Act
        bool result = await _sut.LoginAsync(accountLogin);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Login_ShouldReturnTrue_WhenLoginSucceeds()
    {
        // Arrange
        AccountLogin accountLogin = new()
                                    {
                                        Email = "test@test.com"
                                    };

        _accountRepository.LoginAsync(Arg.Any<AccountLogin>()).Returns(true);

        // Act
        bool result = await _sut.LoginAsync(accountLogin);

        // Assert
        result.Should().BeTrue();
    }
}