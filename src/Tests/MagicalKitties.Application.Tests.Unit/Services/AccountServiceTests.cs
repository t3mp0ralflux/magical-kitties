﻿using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Specialized;
using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Models.System;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ExceptionExtensions;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class AccountServiceTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IValidator<Account> _accountValidator = Substitute.For<IValidator<Account>>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();

    private readonly EquivalencyOptions<Account> _equivalencyOptions;
    private readonly IGlobalSettingsService _globalSettingsService = Substitute.For<IGlobalSettingsService>();
    private readonly ILogger<AccountService> _logger = Substitute.For<ILogger<AccountService>>();
    private readonly IValidator<GetAllAccountsOptions> _optionsValidator = Substitute.For<IValidator<GetAllAccountsOptions>>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IValidator<PasswordReset> _passwordResetValidator = Substitute.For<IValidator<PasswordReset>>();

    public AccountServiceTests()
    {
        _sut = new AccountService(_accountRepository, _accountValidator, _dateTimeProvider, _optionsValidator, _passwordHasher, _globalSettingsService, _emailService, _passwordResetValidator, _logger);

        _equivalencyOptions = new EquivalencyOptions<Account>().Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>();
    }

    public AccountService _sut { get; set; }

    [Fact]
    public async Task CreateAsync_ShouldReturnFalse_WhenAccountIsNotCreated()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();

        _accountRepository.CreateAsync(Arg.Any<Account>()).Returns(false);
        _dateTimeProvider.GetUtcNow().Returns(now);

        // Act
        bool result = await _sut.CreateAsync(account.Clone());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertValidInformationAndQueueEmail_WhenAccountIsCreated()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();
        Account serviceAccount = Fakes.GenerateAccount();

        string testLinkFormat = $"Username: {account.Username}, Password: Test";

        string testEmailFormat = $"Data: {testLinkFormat}";

        _dateTimeProvider.GetUtcNow().Returns(now);
        _accountRepository.CreateAsync(Arg.Any<Account>()).Returns(true);
        _accountRepository.GetByUsernameAsync(serviceAccount.Username).Returns(serviceAccount);
        _passwordHasher.CreateActivationToken().Returns("Test");
        _passwordHasher.Hash(account.Password).Returns("TestHash");

        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.ACTIVATION_EMAIL_FORMAT, string.Empty).Returns(testEmailFormat);
        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.SERVICE_ACCOUNT_USERNAME, string.Empty).Returns(serviceAccount.Username);

        EmailData expectedQueuedEmail = new()
                                        {
                                            Id = Guid.NewGuid(),
                                            ShouldSend = true,
                                            SendAttempts = 0,
                                            SendAfterUtc = now,
                                            SenderAccountId = serviceAccount.Id,
                                            ReceiverAccountId = account.Id,
                                            SenderEmail = serviceAccount.Email,
                                            RecipientEmail = account.Email,
                                            Body = string.Format(testEmailFormat, string.Format(testLinkFormat, account.Username)),
                                            ResponseLog = $"{now}: Email created;"
                                        };

        // Act
        bool result = await _sut.CreateAsync(account.Clone());

        // Assert
        result.Should().BeTrue();
        ICall? createCall = _accountRepository.ReceivedCalls().FirstOrDefault();
        createCall.Should().NotBeNull();

        Account? createdAccount = (Account?)createCall.GetArguments().FirstOrDefault();
        createdAccount.Should().NotBeNull();

        createdAccount.CreatedUtc.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        createdAccount.UpdatedUtc.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));

        ICall? emailCall = _emailService.ReceivedCalls().FirstOrDefault();
        emailCall.Should().NotBeNull();

        EmailData? queuedEmail = (EmailData?)emailCall.GetArguments().FirstOrDefault();
        queuedEmail.Should().NotBeNull();

        queuedEmail.Should().BeEquivalentTo(expectedQueuedEmail, options =>
                                                                 {
                                                                     options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>();
                                                                     options.Excluding(x => x.ResponseLog); // check alone next.
                                                                     options.Excluding(x => x.Id);

                                                                     return options;
                                                                 });

        queuedEmail.ResponseLog.Should().Contain("Email created;");
    }

    [Fact]
    public async Task CreateAsync_ShouldLogError_WhenEmailActivationThrowsError()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();
        Account serviceAccount = Fakes.GenerateAccount();

        _dateTimeProvider.GetUtcNow().Returns(now);
        _accountRepository.CreateAsync(Arg.Any<Account>()).Returns(true);
        _accountRepository.GetByUsernameAsync(serviceAccount.Username, Arg.Any<CancellationToken>()).Returns(serviceAccount);
        _emailService.QueueEmailAsync(Arg.Any<EmailData>()).Throws(new TimeoutException("Db Timeout"));

        // Act
        bool result = await _sut.CreateAsync(account.Clone());

        // Assert
        result.Should().BeTrue();

        IEnumerable<ICall>? loggerCall = _logger.ReceivedCalls();
        loggerCall.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenIdNotFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _accountRepository.GetByIdAsync(id).Returns((Account?)null);

        // Act
        Account? result = await _sut.GetByIdAsync(id.ToString());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAccount_WhenIdIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        _accountRepository.GetByIdAsync(account.Id).Returns(account);

        // Act
        Account? result = await _sut.GetByIdAsync(account.Id.ToString());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(account, _ => _equivalencyOptions);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNothingIsFound()
    {
        // Arrange
        GetAllAccountsOptions options = new()
                                        {
                                            Page = 1,
                                            PageSize = 5
                                        };

        _accountRepository.GetAllAsync(options).Returns([]);

        // Act
        IEnumerable<Account> result = await _sut.GetAllAsync(options);

        // Assert
        List<Account> enumerable = result.ToList();
        enumerable.Should().NotBeNull();
        enumerable.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsList_WhenItemsAreFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        GetAllAccountsOptions options = new()
                                        {
                                            UserName = account.Username,
                                            Page = 1,
                                            PageSize = 5
                                        };

        _accountRepository.GetAllAsync(options).Returns([account]);

        // Act
        Account[] result = (await _sut.GetAllAsync(options)).ToArray();

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result.First().Should().BeEquivalentTo(account, _ => _equivalencyOptions);
    }

    [Fact]
    public async Task GetCountAsync_ReturnsZero_WhenNoItemsAreFound()
    {
        // Arrange
        _accountRepository.GetCountAsync(Arg.Any<string?>()).Returns(0);

        // Act
        int result = await _sut.GetCountAsync("Test");

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(99)]
    public async Task GetCountAsync_ReturnsCount_WhenItemsAreFound(int count)
    {
        // Arrange
        _accountRepository.GetCountAsync(Arg.Any<string?>()).Returns(count);

        // Act
        int result = await _sut.GetCountAsync("Test");

        // Assert
        result.Should().Be(count);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenNoEmailIsSupplied()
    {
        // Arrange
        _accountRepository.GetByEmailAsync(string.Empty).Returns((Account?)null);

        // Act
        Account? result = await _sut.GetByEmailAsync(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenEmailNotFound()
    {
        // Arrange
        _accountRepository.GetByEmailAsync("test@test.com").Returns((Account?)null);

        // Act
        Account? result = await _sut.GetByEmailAsync("test@test.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsAccount_WhenEmailIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        _accountRepository.GetByEmailAsync(account.Email).Returns(account);

        // Act
        Account? result = await _sut.GetByEmailAsync(account.Email);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(account, _ => _equivalencyOptions);
    }

    [Fact]
    public async Task GetByUsernameAsync_ReturnsNull_WhenUsernameNotFound()
    {
        // Arrange
        _accountRepository.GetByUsernameAsync("test").Returns((Account?)null);

        // Act
        Account? result = await _sut.GetByUsernameAsync("test");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsernameAsync_ReturnsAccount_WhenUsernameIsFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);

        // Act
        Account? result = await _sut.GetByUsernameAsync(account.Username);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(account, _ => _equivalencyOptions);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenAccountIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        _accountRepository.ExistsByIdAsync(account.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Account? result = await _sut.UpdateAsync(account);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsAccount_WhenUpdateIsSuccessful()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();

        _accountRepository.ExistsByIdAsync(account.Id, Arg.Any<CancellationToken>()).Returns(true);
        _accountRepository.UpdateAsync(account, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Account? result = await _sut.UpdateAsync(account, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(account.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenIdIsNotFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _accountRepository.ExistsByIdAsync(id).Returns(false);

        // Act
        bool result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenIdIsFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _accountRepository.ExistsByIdAsync(id).Returns(true);
        _accountRepository.DeleteAsync(id).Returns(true);

        // Act
        bool result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ActivateAsync_ShouldThrowValidationException_WhenAccountIsNotFound()
    {
        // Arrange
        AccountActivation activation = new()
                                       {
                                           Username = "Test",
                                           ActivationCode = "Test Code",
                                           Expiration = DateTime.UtcNow
                                       };

        // Act
        Func<Task<bool>> action = async () => await _sut.ActivateAsync(activation);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("Account");
        error.ErrorMessage.Should().Be("No Account found");
    }

    [Fact]
    public async Task ActivateAsync_ShouldThrowValidationException_WhenActivationCodeIsNotValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        account.ActivationCode = "Old and busted";

        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);

        AccountActivation activation = new()
                                       {
                                           Username = account.Username,
                                           ActivationCode = "New Hotness",
                                           Expiration = DateTime.UtcNow
                                       };

        // Act
        Func<Task<bool>> action = async () => await _sut.ActivateAsync(activation);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("ActivationCode");
        error.ErrorMessage.Should().Be("Activation is invalid");
    }

    [Fact]
    public async Task ActivateAsync_ShouldThrowValidationException_WhenActivationExpirationIsNotValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount().WithActivation();

        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);
        _dateTimeProvider.GetUtcNow().Returns(DateTime.UtcNow);

        AccountActivation activation = new()
                                       {
                                           Username = account.Username,
                                           ActivationCode = account.ActivationCode!,
                                           Expiration = DateTime.MinValue
                                       };

        // Act
        Func<Task<bool>> action = async () => await _sut.ActivateAsync(activation);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("ActivationCode");
        error.ErrorMessage.Should().Be("Activation is invalid");
    }

    [Fact]
    public async Task ActivateAsync_ShouldReturnFalse_WhenActivationFailsInDb()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        account.ActivationCode = "Test";

        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);
        _accountRepository.ActivateAsync(Arg.Any<Account>()).Returns(false);

        AccountActivation activation = new()
                                       {
                                           Username = account.Username,
                                           ActivationCode = account.ActivationCode,
                                           Expiration = DateTime.MinValue
                                       };

        // Act
        bool result = await _sut.ActivateAsync(activation);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ActivateAsync_ShouldThrowValidationException_WhenAccountIsAlreadyActive()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        DateTime now = DateTime.UtcNow;
        account.ActivatedUtc = now;
        _dateTimeProvider.GetUtcNow().Returns(now);
        
        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);
        
        AccountActivation activation = new()
                                       {
                                           Username = account.Username,
                                           ActivationCode = "test", // doesn't matter
                                           Expiration = DateTime.MinValue
                                       };
        
        // Act
        Func<Task<bool>> action = async () => await _sut.ActivateAsync(activation);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("Account");
        error.ErrorMessage.Should().Be("Activation is invalid");
    }

    [Fact]
    public async Task ActivateAsync_ShouldReturnTrue_WhenActivationSucceeds()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        account.ActivationCode = "Test";

        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);
        _accountRepository.ActivateAsync(Arg.Any<Account>()).Returns(true);

        AccountActivation activation = new()
                                       {
                                           Username = account.Username,
                                           ActivationCode = account.ActivationCode,
                                           Expiration = DateTime.MinValue
                                       };

        // Act
        bool result = await _sut.ActivateAsync(activation);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ResendActivationAsync_ShouldThrowValidationException_WhenAccountIsNotFound()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        AccountActivation request = new()
                                    {
                                        Username = account.Username,
                                        ActivationCode = "Old and busted",
                                        Expiration = DateTime.MinValue
                                    };

        _accountRepository.GetByUsernameAsync(account.Username).Returns((Account?)null);

        // Act
        Func<Task<bool>> action = async () => await _sut.ResendActivationAsync(request);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("Account");
        error.ErrorMessage.Should().Be("No account found");
    }

    [Fact]
    public async Task ResendActivationAsync_ShouldThrowValidationException_WhenActivationCodeIsInvalid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        account.ActivationCode = "Old and busted";

        AccountActivation request = new()
                                    {
                                        Username = account.Username,
                                        ActivationCode = "New Hotness",
                                        Expiration = DateTime.MinValue
                                    };

        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);

        // Act
        Func<Task<bool>> action = async () => await _sut.ResendActivationAsync(request);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("ActivationCode");
        error.ErrorMessage.Should().Be("Activation code invalid");
    }
    
    [Fact]
    public async Task ResendActivationAsync_ShouldThrowValidationException_WhenActivationCodeIsStillValid()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        account.ActivationCode = "Old and busted";
        account.ActivationExpiration = DateTime.Now.AddMinutes(5);

        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);
        
        AccountActivation request = new()
                                    {
                                        Username = account.Username,
                                        ActivationCode = "Old and busted",
                                        Expiration = DateTime.MinValue
                                    };

        // Act
        Func<Task<bool>> action = async () => await _sut.ResendActivationAsync(request);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("ActivationExpiration");
        error.ErrorMessage.Should().Be("Activation code active");
    }

    [Fact]
    public async Task ResendActivationAsync_ShouldReturnFalse_WhenUpdateActivationAsyncFails()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        account.ActivationCode = "Test";

        AccountActivation request = new()
                                    {
                                        Username = account.Username,
                                        ActivationCode = account.ActivationCode,
                                        Expiration = DateTime.MinValue
                                    };

        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);
        _accountRepository.UpdateActivationAsync(Arg.Any<Account>()).Returns(false);

        // Act
        bool result = await _sut.ResendActivationAsync(request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ResendActivationAsync_ShouldInsertValidInformationAndQueueEmail_WhenAccountIsCreated()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();
        Account serviceAccount = Fakes.GenerateAccount();

        account.ActivationCode = "Test";

        AccountActivation request = new()
                                    {
                                        Username = account.Username,
                                        ActivationCode = account.ActivationCode,
                                        Expiration = DateTime.UtcNow
                                    };

        string testLinkFormat = $"Username: {account.Username}, Password: Test";

        string testEmailFormat = $"Data: {testLinkFormat}";

        _dateTimeProvider.GetUtcNow().Returns(now);
        _accountRepository.UpdateActivationAsync(Arg.Any<Account>()).Returns(true);
        _accountRepository.GetByUsernameAsync(serviceAccount.Username).Returns(serviceAccount);
        _accountRepository.GetByUsernameAsync(account.Username).Returns(account);

        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.ACTIVATION_EMAIL_FORMAT, string.Empty).Returns(testEmailFormat);
        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.SERVICE_ACCOUNT_USERNAME, string.Empty).Returns(serviceAccount.Username);

        EmailData expectedQueuedEmail = new()
                                        {
                                            Id = Guid.NewGuid(),
                                            ShouldSend = true,
                                            SendAttempts = 0,
                                            SendAfterUtc = now,
                                            SenderAccountId = serviceAccount.Id,
                                            ReceiverAccountId = account.Id,
                                            SenderEmail = serviceAccount.Email,
                                            RecipientEmail = account.Email,
                                            Body = string.Format(testEmailFormat, string.Format(testLinkFormat, account.Username, "Test")),
                                            ResponseLog = $"{now}: Email created;"
                                        };

        // Act
        bool result = await _sut.ResendActivationAsync(request);

        // Assert
        result.Should().BeTrue();

        ICall? emailCall = _emailService.ReceivedCalls().FirstOrDefault();
        emailCall.Should().NotBeNull();

        EmailData? queuedEmail = (EmailData?)emailCall.GetArguments().FirstOrDefault();
        queuedEmail.Should().NotBeNull();

        queuedEmail.Should().BeEquivalentTo(expectedQueuedEmail, options =>
                                                                 {
                                                                     options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>();
                                                                     options.Excluding(x => x.ResponseLog); // check alone next.
                                                                     options.Excluding(x => x.Id);

                                                                     return options;
                                                                 });

        queuedEmail.ResponseLog.Should().Contain("Email created;");
    }

    [Fact]
    public async Task ResendActivationAsync_ShouldLogError_WhenEmailActivationThrowsError()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();
        Account serviceAccount = Fakes.GenerateAccount();

        account.ActivationCode = "Test";

        AccountActivation request = new()
                                    {
                                        Username = account.Username,
                                        ActivationCode = account.ActivationCode,
                                        Expiration = DateTime.UtcNow
                                    };

        _dateTimeProvider.GetUtcNow().Returns(now);
        _accountRepository.UpdateActivationAsync(Arg.Any<Account>()).Returns(true);
        _accountRepository.GetByUsernameAsync(serviceAccount.Username, Arg.Any<CancellationToken>()).Returns(serviceAccount);
        _accountRepository.GetByUsernameAsync(account.Username, Arg.Any<CancellationToken>()).Returns(account);
        _emailService.QueueEmailAsync(Arg.Any<EmailData>()).Throws(new TimeoutException("Db Timeout"));

        // Act
        bool result = await _sut.ResendActivationAsync(request);

        // Assert
        result.Should().BeTrue();

        IEnumerable<ICall>? loggerCall = _logger.ReceivedCalls();
        loggerCall.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnFalse_WhenAccountDoesNotExist()
    {
        // Arrange
        const string email = "email@email.com";

        _accountRepository.GetByEmailAsync(email).Returns((Account?)null);

        // Act
        bool result = await _sut.RequestPasswordReset(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnTrueAndNotUpdate_WhenRecentRequestExists()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();

        account.PasswordResetRequestedUtc = now;

        _dateTimeProvider.GetUtcNow().Returns(now);
        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_REQUEST_DURATION_MINS, 5).Returns(5);

        _accountRepository.GetByEmailAsync(account.Email).Returns(account);

        // Act
        bool result = await _sut.RequestPasswordReset(account.Email);

        // Assert
        result.Should().BeTrue();
        await _accountRepository.DidNotReceive().RequestPasswordResetAsync(account.Email, Arg.Any<string>());
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnTrueAndUpdate_WhenRequestIsExpired()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();

        account.PasswordResetRequestedUtc = now.AddMinutes(-10);

        _dateTimeProvider.GetUtcNow().Returns(now);
        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_REQUEST_DURATION_MINS, 5).Returns(5);
        _passwordHasher.CreateOneTimeCode().Returns("069420");

        _accountRepository.GetByEmailAsync(account.Email).Returns(account);

        // Act
        bool result = await _sut.RequestPasswordReset(account.Email);

        // Assert
        result.Should().BeTrue();
        await _accountRepository.Received().RequestPasswordResetAsync(account.Email, "069420");
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnTrueAndUpdate_WhenNoRequestExists()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        Account account = Fakes.GenerateAccount();

        _dateTimeProvider.GetUtcNow().Returns(now);
        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_REQUEST_DURATION_MINS, 5).Returns(5);
        _passwordHasher.CreateOneTimeCode().Returns("069420");

        _accountRepository.GetByEmailAsync(account.Email).Returns(account);

        // Act
        bool result = await _sut.RequestPasswordReset(account.Email);

        // Assert
        result.Should().BeTrue();
        await _accountRepository.Received().RequestPasswordResetAsync(account.Email, "069420");
    }

    [Fact]
    public async Task VerifyPasswordResetCode_ShouldThrowValidationException_WhenResetNotRequested()
    {
        // Arrange
        const string email = "test@test.com";
        const string code = "1235";

        // Act
        Func<Task<bool>> action = async () => await _sut.VerifyPasswordResetCodeAsync(email, code);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("Reset");
        error.ErrorMessage.Should().Be("Reset was not requested");
    }

    [Fact]
    public async Task VerifyPasswordResetCode_ShouldThrowValidationException_WhenResetCodeHasExpired()
    {
        // Arrange
        const string code = "12345";
        DateTime now = DateTime.UtcNow;

        Account account = Fakes.GenerateAccount();
        account.PasswordResetCode = "12345";
        account.PasswordResetRequestedUtc = now.AddHours(-1);

        _accountRepository.GetByEmailAsync(account.Email).Returns(account);

        _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_RESET_REQUEST_EXPIRATION_MINS, 5).Returns(5);
        _dateTimeProvider.GetUtcNow().Returns(now);

        // Act
        Func<Task<bool>> action = async () => await _sut.VerifyPasswordResetCodeAsync(account.Email, code);

        // Assert
        ExceptionAssertions<ValidationException>? errorResult = await action.Should().ThrowAsync<ValidationException>();

        ValidationException? exception = errorResult.Subject.FirstOrDefault();
        exception.Should().NotBeNull();
        exception.Errors.Should().NotBeEmpty();

        ValidationFailure? error = exception.Errors.First();
        error.PropertyName.Should().Be("ResetCode");
        error.ErrorMessage.Should().Be("Code has expired");
    }
}