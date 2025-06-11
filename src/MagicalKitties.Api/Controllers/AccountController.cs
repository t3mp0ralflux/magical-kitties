using Asp.Versioning;
using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Account;
using MagicalKitties.Contracts.Responses.Account;
using MagicalKitties.Contracts.Responses.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiVersion(1.0)]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost(ApiEndpoints.Accounts.Create)]
    [ProducesResponseType<AccountResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AccountCreateRequest request, CancellationToken token)
    {
        Account account = request.ToAccount();
        await accountService.CreateAsync(account, token);

        AccountResponse response = account.ToResponse();

        return CreatedAtAction(nameof(Get), new { emailOrId = account.Id }, response);
    }

    [HttpGet(ApiEndpoints.Accounts.Get)]
    [ProducesResponseType<AccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string emailOrId, CancellationToken token)
    {
        Account? account = await accountService.GetByIdAsync(emailOrId, token);

        if (account is null)
        {
            return NotFound();
        }

        AccountResponse response = account.ToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Accounts.GetAll)]
    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [ProducesResponseType<AccountsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllAccountsRequest request, CancellationToken token)
    {
        GetAllAccountsOptions options = request.ToOptions();
        IEnumerable<Account> result = await accountService.GetAllAsync(options, token);
        int accountCount = await accountService.GetCountAsync(options.UserName, token);

        AccountsResponse response = result.ToResponse(request.Page, request.PageSize, accountCount);

        return Ok(response);
    }

    [HttpPut(ApiEndpoints.Accounts.Update)]
    [ProducesResponseType<AccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AccountUpdateRequest request, CancellationToken token)
    {
        Account account = request.ToAccount(id);
        Account? result = await accountService.UpdateAsync(account, token);

        if (result is null)
        {
            return NotFound();
        }

        AccountResponse response = account.ToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Accounts.Delete)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        bool deleted = await accountService.DeleteAsync(id, token);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet(ApiEndpoints.Accounts.Activate)]
    [ProducesResponseType<AccountActivationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Activate([FromRoute] string username, [FromRoute] string activationcode, CancellationToken token)
    {
        AccountActivation activationRequest = new()
                                              {
                                                  ActivationCode = activationcode,
                                                  Username = username,
                                                  Expiration = DateTime.MinValue
                                              };

        // throws validation errors
        await accountService.ActivateAsync(activationRequest, token);

        AccountActivationResponse response = activationRequest.ToResponse();

        return Ok(response);
    }

    [HttpPost(ApiEndpoints.Accounts.ResendActivation)]
    [ProducesResponseType<AccountActivationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendActivation([FromRoute] string username, [FromRoute] string activationCode, CancellationToken token)
    {
        AccountActivation activationRequest = new()
                                              {
                                                  ActivationCode = activationCode,
                                                  Username = username,
                                                  Expiration = DateTime.MinValue
                                              };
        bool resendActivationResult = await accountService.ResendActivationAsync(activationRequest, token);

        // validations throw exceptions. False means account wasn't found.
        if (!resendActivationResult)
        {
            return NotFound();
        }

        AccountActivationResponse response = activationRequest.ToResponse();

        return Ok(response);
    }
}