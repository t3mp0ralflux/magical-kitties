﻿using FluentValidation;
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
public class AuthController(IAccountService accountService, IPasswordHasher passwordHasher, IJwtTokenGeneratorService jwtTokenGeneratorService, IAuthService authService)
    : ControllerBase
{
    [HttpPost(ApiEndpoints.Auth.Login)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UnauthorizedObjectResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken token)
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

        AccountLogin accountLogin = account.ToLogin();

        await authService.LoginAsync(accountLogin, token);

        string jwtToken = jwtTokenGeneratorService.GenerateToken(account);

        return Ok(jwtToken);
    }

    [HttpPost(ApiEndpoints.Auth.RequestPasswordReset)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestPasswordReset([FromRoute] string email, CancellationToken token)
    {
        await accountService.RequestPasswordReset(email, token); // DO NOT SURFACE TO USER. If the account isn't found, it'll fail in silence.

        return Ok(email);
    }

    [HttpPost(ApiEndpoints.Auth.VerifyPasswordResetCode)]
    [ProducesResponseType<OkResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPasswordResetCode([FromRoute] string email, [FromBody] PasswordResetVerification verification, CancellationToken token)
    {
        if (!string.Equals(email.ToLowerInvariant(), verification.Email.ToLowerInvariant()))
        {
            throw new ValidationException("Email not valid");
        }

        // throws ValidationExceptions if not valid
        await accountService.VerifyPasswordResetCode(verification.Email, verification.Code, token);

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

        await accountService.ResetPassword(passwordReset, token);

        PasswordResetResponse response = passwordReset.ToResponse();

        return Ok(response);
    }
}