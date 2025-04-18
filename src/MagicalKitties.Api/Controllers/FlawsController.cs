﻿using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Endowments.Flaws;
using MagicalKitties.Contracts.Responses.Characters;
using MagicalKitties.Contracts.Responses.Flaws;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class FlawsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IFlawService _flawService;

    public FlawsController(IAccountService accountService, IFlawService flawService)
    {
        _accountService = accountService;
        _flawService = flawService;
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPost(ApiEndpoints.Flaws.Create)]
    [ProducesResponseType<EndowmentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateFlawRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Flaw result = request.ToFlaw();

        await _flawService.CreateAsync(result, token);

        FlawResponse response = result.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    [HttpGet(ApiEndpoints.Flaws.Get)]
    [ProducesResponseType<FlawResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Flaw? result = await _flawService.GetByIdAsync(id, token);

        if (result is null)
        {
            return NotFound();
        }

        FlawResponse response = result.ToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Flaws.GetAll)]
    [ProducesResponseType<FlawsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(GetAllFlawsRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        GetAllFlawsOptions options = request.ToOptions();

        IEnumerable<Flaw> results = await _flawService.GetAllAsync(options, token);
        int total = await _flawService.GetCountAsync(options, token);

        FlawsResponse response = results.ToResponse(options.Page, options.PageSize, total);

        return Ok(response);
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPut(ApiEndpoints.Flaws.Update)]
    [ProducesResponseType<FlawResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(UpdateFlawRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Flaw flaw = request.ToFlaw();

        bool result = await _flawService.UpdateAsync(flaw, token);

        if (!result)
        {
            return NotFound();
        }

        FlawResponse response = flaw.ToResponse();

        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Flaws.Delete)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool result = await _flawService.DeleteAsync(id, token);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}