﻿using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Endowments.Talents;
using MagicalKitties.Contracts.Responses.Talents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class TalentsController(IAccountService accountService, ITalentService talentService, IOutputCacheStore outputCacheStore) : ControllerBase
{
    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPost(ApiEndpoints.Talents.Create)]
    [ProducesResponseType<TalentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateTalentRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Talent result = request.ToTalent();

        await talentService.CreateAsync(result, token);

        await outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.Talents, token);

        TalentResponse response = result.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    [HttpGet(ApiEndpoints.Talents.Get)]
    [OutputCache(PolicyName = ApiAssumptions.PolicyNames.Talents)]
    [ProducesResponseType<TalentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        Talent? result = await talentService.GetByIdAsync(id, token);

        if (result is null)
        {
            return NotFound();
        }

        TalentResponse response = result.ToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Talents.GetAll)]
    [OutputCache(PolicyName = ApiAssumptions.PolicyNames.Talents)]
    [ProducesResponseType<TalentsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(GetAllTalentsRequest request, CancellationToken token)
    {
        GetAllTalentsOptions options = request.ToOptions();

        IEnumerable<Talent> results = await talentService.GetAllAsync(options, token);
        int total = await talentService.GetCountAsync(options, token);

        TalentsResponse response = results.ToResponse(options.Page, options.PageSize, total);

        return Ok(response);
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPut(ApiEndpoints.Talents.Update)]
    [ProducesResponseType<TalentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(UpdateTalentRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Talent talent = request.ToTalent();

        bool result = await talentService.UpdateAsync(talent, token);

        if (!result)
        {
            return NotFound();
        }

        TalentResponse response = talent.ToResponse();

        await outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.Talents, token);
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Talents.Delete)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool result = await talentService.DeleteAsync(id, token);

        if (!result)
        {
            return NotFound();
        }

        await outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.Talents, token);
        return NoContent();
    }
}