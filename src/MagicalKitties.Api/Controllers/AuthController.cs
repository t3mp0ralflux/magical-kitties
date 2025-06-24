using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using FluentValidation.Results;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Api.Services;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Auth;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Auth;
using MagicalKitties.Contracts.Responses.Auth;
using MagicalKitties.Contracts.Responses.Errors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class AuthController(IAccountService accountService, IRefreshTokenService refreshTokenService, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, IAuthService authService)
    : ControllerBase
{
    [HttpPost(ApiEndpoints.Auth.Login)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UnauthorizedObjectResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginByPassword([FromBody] LoginRequest request, CancellationToken token)
    {
        Account? account;
        if (request.Email.Contains('@'))
        {
            account = await accountService.GetByEmailAsync(request.Email, token);
        }
        else
        {
            account = await accountService.GetByUsernameAsync(request.Email, token);
        }

        if (account is null)
        {
            return NotFound();
        }

        if (account.AccountStatus != AccountStatus.active)
        {
            return Unauthorized("You must activate your account before you can login");
        }

        bool verified = passwordHasher.Verify(request.Password, account.Password);

        if (!verified)
        {
            return Unauthorized("Username or password is incorrect");
        }

        string accessToken = jwtTokenService.GenerateToken(account);
        string refreshToken = jwtTokenService.GenerateRefreshToken();

        // create that new refresh token
        await refreshTokenService.UpsertRefreshToken(account, accessToken, refreshToken, token);
        
        AccountLogin accountLogin = account.ToLogin();
        await authService.LoginAsync(accountLogin, token);

        LoginResponse response = new LoginResponse
                                 {
                                     Account = account.ToResponse(),
                                     AccessToken = accessToken,
                                     RefreshToken = refreshToken
                                 };
        
        return Ok(response);
    }
    
    [HttpPost(ApiEndpoints.Auth.LoginByToken)]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<UnauthorizedObjectResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginByToken(TokenRequest request, CancellationToken token)
    {
        AuthToken authToken = request.ToToken();
        bool tokenIsValid = jwtTokenService.ValidateCustomToken(authToken.AccessToken);

        if (!tokenIsValid)
        {
            return Unauthorized("Token is invalid");
        }
        
        string? tokenEmail = jwtTokenService.GetEmailFromToken(authToken.AccessToken);

        if (string.IsNullOrWhiteSpace(tokenEmail))
        {
            return Unauthorized("Token is invalid");
        }
        
        Account? account = await accountService.GetByEmailAsync(tokenEmail, token);

        if (account is null)
        {
            // TODO: add in SSO login logic here
            return NotFound();
        }

        if (account.AccountStatus != AccountStatus.active)
        {
            return Unauthorized("Your account status is not active. Contact support.");
        }

        bool existingToken = await refreshTokenService.Exists(account.Id, token);

        if (!existingToken)
        {
            return Unauthorized("Refresh token is invalid");
        }
        
        bool validToken = await refreshTokenService.ValidateRefreshToken(account.Id, authToken, token);

        if (!validToken)
        {
            return Unauthorized("Refresh token is invalid");
        }

        string newAccessToken = jwtTokenService.GenerateToken(account);
        string newRefreshToken = jwtTokenService.GenerateRefreshToken();
        
        RefreshToken responseToken = await refreshTokenService.UpsertRefreshToken(account, newAccessToken, newRefreshToken, token);
        
        AccountLogin accountLogin = account.ToLogin();

        await authService.LoginAsync(accountLogin, token);

        LoginResponse response = new LoginResponse
                                 {
                                     Account = account.ToResponse(),
                                     AccessToken = newAccessToken,
                                     RefreshToken = responseToken.Token
                                 };

        return Ok(response);
    }

    [HttpPost(ApiEndpoints.Auth.Logout)]
    [ProducesResponseType<OkResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout([FromRoute] Guid accountId, CancellationToken token)
    {
        Account? account = await accountService.GetByIdAsync(accountId.ToString(), token);

        if (account is null)
        {
            return NotFound();
        }

        await refreshTokenService.DeleteRefreshToken(accountId, token);

        return Ok();
    }

    [HttpPost(ApiEndpoints.Auth.RequestPasswordReset)]
    [ProducesResponseType<PasswordResetResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request, CancellationToken token)
    {
        await accountService.RequestPasswordReset(request.Email, token); // DO NOT SURFACE TO USER. If the account isn't found, it'll fail in silence.

        PasswordResetResponse response = new PasswordResetResponse { Email = request.Email };

        return Ok(response);
    }
    
    [HttpPost(ApiEndpoints.Auth.VerifyPasswordResetCode)]
    [ProducesResponseType<OkResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPasswordResetCode([FromBody] PasswordResetRequest verification, CancellationToken token)
    {
        // throws ValidationExceptions if not valid
        await accountService.VerifyPasswordResetCodeAsync(verification.Email, verification.ResetCode, token);

        return Ok();
    }

    [HttpPost(ApiEndpoints.Auth.PasswordReset)]
    [ProducesResponseType<PasswordResetResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PasswordReset([FromBody] PasswordResetRequest request, CancellationToken token)
    {
        bool accountExists = await accountService.ExistsByEmailAsync(request.Email, token);

        if (!accountExists)
        {
            return NotFound();
        }

        PasswordReset passwordReset = request.ToReset();

        await accountService.ResetPasswordAsync(passwordReset, token);

        PasswordResetResponse response = passwordReset.ToResponse();

        return Ok(response);
    }
}