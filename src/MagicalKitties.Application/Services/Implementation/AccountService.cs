using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Models.System;
using MagicalKitties.Application.Repositories;
using Microsoft.Extensions.Logging;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace MagicalKitties.Application.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IValidator<Account> _accountValidator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailService _emailService;
    private readonly IGlobalSettingsService _globalSettingsService;
    private readonly IValidator<GetAllAccountsOptions> _optionsValidator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<PasswordReset> _passwordResetValidator;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, IValidator<Account> accountValidator, IDateTimeProvider dateTimeProvider, IValidator<GetAllAccountsOptions> optionsValidator, IPasswordHasher passwordHasher, IGlobalSettingsService globalSettingsService, IEmailService emailService, IValidator<PasswordReset> passwordResetValidator, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _accountValidator = accountValidator;
        _optionsValidator = optionsValidator;
        _passwordResetValidator = passwordResetValidator;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _globalSettingsService = globalSettingsService;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> CreateAsync(Account account, CancellationToken token = default)
    {
        await _accountValidator.ValidateAndThrowAsync(account, token);
        account.CreatedUtc = _dateTimeProvider.GetUtcNow();
        account.UpdatedUtc = _dateTimeProvider.GetUtcNow();
        account.Password = _passwordHasher.Hash(account.Password);

        await CreateActivationData(account, token);

        bool success = await _accountRepository.CreateAsync(account, token);

        if (!success)
        {
            return false;
        }

        try
        {
            await QueueActivationEmailAsync(account, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
        }

        return true;
    }

    public async Task<Account?> GetByIdAsync(string emailOrId, CancellationToken token = default)
    {
        if (Guid.TryParse(emailOrId, out Guid guidId))
        {
            return await _accountRepository.GetByIdAsync(guidId, token);
        }
        
        return await _accountRepository.GetByEmailAsync(emailOrId, token);
    }

    public async Task<IEnumerable<Account>> GetAllAsync(GetAllAccountsOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);

        return await _accountRepository.GetAllAsync(options, token);
    }

    public async Task<int> GetCountAsync(string? username, CancellationToken token = default)
    {
        return await _accountRepository.GetCountAsync(username, token);
    }

    public async Task<Account?> GetByEmailAsync(string? email, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return await _accountRepository.GetByEmailAsync(email, token);
    }

    public async Task<Account?> GetByUsernameAsync(string username, CancellationToken token = default)
    {
        return await _accountRepository.GetByUsernameAsync(username, token);
    }

    public async Task<Account?> UpdateAsync(Account account, CancellationToken token = default)
    {
        bool existingAccount = await _accountRepository.ExistsByIdAsync(account.Id, token);
        if (!existingAccount)
        {
            return null;
        }

        await _accountRepository.UpdateAsync(account, token);
        return account;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
    {
        return await _accountRepository.DeleteAsync(id, token);
    }

    public async Task<bool> ActivateAsync(AccountActivation activation, CancellationToken token = default)
    {
        Account? existingAccount = await _accountRepository.GetByUsernameAsync(activation.Username, token);
        
        if (existingAccount is null)
        {
            throw new ValidationException([new ValidationFailure("Account", "No Account found")]);
        }

        // don't re-activate for no reason
        if (existingAccount.ActivatedUtc.HasValue)
        {
            return true;
        }

        if (existingAccount.ActivationCode != activation.ActivationCode || existingAccount.ActivationExpiration < _dateTimeProvider.GetUtcNow())
        {
            throw new ValidationException([new ValidationFailure("ActivationCode","Activation is invalid")]);
        }
        
        existingAccount.AccountStatus = AccountStatus.active;
        existingAccount.ActivatedUtc = _dateTimeProvider.GetUtcNow();
        existingAccount.UpdatedUtc = _dateTimeProvider.GetUtcNow();
        
        return await _accountRepository.ActivateAsync(existingAccount, token);;
    }

    public async Task<bool> ResendActivationAsync(AccountActivation activationRequest, CancellationToken token = default)
    {
        Account? account = await _accountRepository.GetByUsernameAsync(activationRequest.Username, token);

        if (account is null)
        {
            throw new ValidationException([new ValidationFailure("Account","No account found")]);
        }

        if (!string.Equals(account.ActivationCode, activationRequest.ActivationCode))
        {
            throw new ValidationException([new ValidationFailure("ActivationCode","Activation code invalid")]);
        }

        if (account.ActivationExpiration > _dateTimeProvider.GetUtcNow())
        {
            throw new ValidationException([new ValidationFailure("ActivationExpiration", "Activation code active")]);
        }
        
        await CreateActivationData(account, token);

        bool success = await _accountRepository.UpdateActivationAsync(account, token);

        if (!success)
        {
            return false;
        }

        try
        {
            await QueueActivationEmailAsync(account, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message}", e.Message);
        }

        return success;
    }
    
    public async Task<bool> ExistsByEmailAsync(string? email, CancellationToken token = default)
    {
        if (email is null)
        {
            return false;
        }

        return await _accountRepository.ExistsByEmailAsync(email, token);
    }

    public async Task<bool> RequestPasswordReset(string email, CancellationToken token = default)
    {
        Account? account = await _accountRepository.GetByEmailAsync(email, token);

        if (account is null)
        {
            return false;
        }

        // already requested at one point
        if (account.PasswordResetRequestedUtc.HasValue)
        {
            TimeSpan duration = (_dateTimeProvider.GetUtcNow() - account.PasswordResetRequestedUtc.Value).Duration();
            int maxPasswordResetMinutes = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_REQUEST_DURATION_MINS, 5, token);

            if (duration < TimeSpan.FromMinutes(maxPasswordResetMinutes))
            {
                return true; // do nothing as they're still in the time window 
            }
        }

        account.PasswordResetCode = _passwordHasher.CreateOneTimeCode();

        await _accountRepository.RequestPasswordResetAsync(email, account.PasswordResetCode, token);

        try
        {
            await QueuePasswordResetEmailAsync(account, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
        }

        return true;
    }

    public async Task<bool> VerifyPasswordResetCodeAsync(string email, string code, CancellationToken token = default)
    {
        Account? account = await _accountRepository.GetByEmailAsync(email, token);

        if (account?.PasswordResetRequestedUtc == null)
        {
            throw new ValidationException([new ValidationFailure("Reset","Reset was not requested")]);
        }

        int maxPasswordResetMins = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_RESET_REQUEST_EXPIRATION_MINS, 5, token);

        if (
            (_dateTimeProvider.GetUtcNow() - account.PasswordResetRequestedUtc.Value).Duration() > TimeSpan.FromMinutes(maxPasswordResetMins)
            || !string.Equals(account.PasswordResetCode, code))
        {
            throw new ValidationException([new ValidationFailure("ResetCode","Code has expired")]);
        }

        return true;
    }

    public async Task<bool> ResetPasswordAsync(PasswordReset reset, CancellationToken token = default)
    {
        await _passwordResetValidator.ValidateAndThrowAsync(reset, token);

        reset.Password = _passwordHasher.Hash(reset.Password); // gotta have it

        bool result = await _accountRepository.ResetPasswordAsync(reset, token);

        return result;
    }

    private async Task QueueActivationEmailAsync(Account account, CancellationToken token = default)
    {
        string? serviceUsername = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.SERVICE_ACCOUNT_USERNAME, string.Empty, token);
        string? emailFormat = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.ACTIVATION_EMAIL_FORMAT, string.Empty, token);
        int expirationMinutes = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.ACCOUNT_ACTIVATION_EXPIRATION_MINS, 5, token);

        Account? serviceAccount = await _accountRepository.GetByUsernameAsync(serviceUsername, token);

        if (serviceAccount is null)
        {
            _logger.LogError("Service Account was not found to queue activation email");
            return;
        }

        if (emailFormat is null)
        {
            _logger.LogError("Activation Email Format was not found. Cannot send email");
            return;
        }

        string activationLink = string.Format(ApplicationAssumptions.ActivationLinkFormat, account.Username, account.ActivationCode);
        string resendLink = string.Format(ApplicationAssumptions.ResendActivationLinkFormat, account.Username, account.ActivationCode);

        EmailData data = new()
                         {
                             Id = Guid.NewGuid(),
                             ShouldSend = true,
                             SendAttempts = 0,
                             SendAfterUtc = _dateTimeProvider.GetUtcNow(),
                             SenderAccountId = serviceAccount.Id,
                             ReceiverAccountId = account.Id,
                             SenderEmail = serviceAccount.Email,
                             RecipientEmail = account.Email,
                             Body = string.Format(emailFormat, activationLink, resendLink, expirationMinutes),
                             ResponseLog = $"{_dateTimeProvider.GetUtcNow()}: Email created;"
                         };

        _emailService.QueueEmailAsync(data, token); // fire and forget, no waiting.
    }

    private async Task QueuePasswordResetEmailAsync(Account account, CancellationToken token = default)
    {
        string? serviceUsername = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.SERVICE_ACCOUNT_USERNAME, string.Empty, token);
        string? emailFormat = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_RESET_EMAIL_FORMAT, string.Empty, token);
        int expirationMinutes = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.PASSWORD_RESET_REQUEST_EXPIRATION_MINS, 5, token);

        Account? serviceAccount = await _accountRepository.GetByUsernameAsync(serviceUsername, token);

        if (serviceAccount is null)
        {
            _logger.LogError("Service Account was not found to queue password reset email");
            return;
        }

        if (emailFormat is null)
        {
            _logger.LogError("Password Reset email format was not found. Cannot send email");
            return;
        }

        EmailData data = new()
                         {
                             Id = Guid.NewGuid(),
                             ShouldSend = true,
                             SendAttempts = 0,
                             SendAfterUtc = _dateTimeProvider.GetUtcNow(),
                             SenderAccountId = serviceAccount.Id,
                             ReceiverAccountId = account.Id,
                             SenderEmail = serviceAccount.Email,
                             RecipientEmail = account.Email,
                             Body = string.Format(emailFormat, account.PasswordResetCode, expirationMinutes),
                             ResponseLog = $"{_dateTimeProvider.GetUtcNow()}: Email created;"
                         };

        _emailService.QueueEmailAsync(data, token); // fire and forget, no waiting.
    }

    private async Task CreateActivationData(Account account, CancellationToken token)
    {
        int expirationMinutes = await _globalSettingsService.GetSettingCachedAsync(WellKnownGlobalSettings.ACCOUNT_ACTIVATION_EXPIRATION_MINS, 5, token);

        account.ActivationExpiration = _dateTimeProvider.GetUtcNow().AddMinutes(expirationMinutes);
        account.ActivationCode = _passwordHasher.CreateActivationToken();
    }
}