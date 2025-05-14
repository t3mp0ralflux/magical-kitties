using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;
using MagicalKitties.Contracts.Responses.MagicalPowers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class MagicalPowersController(IAccountService accountService, IMagicalPowerService magicalPowerService, IOutputCacheStore outputCacheStore) : ControllerBase
{
    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPost(ApiEndpoints.MagicalPowers.Create)]
    [ProducesResponseType<MagicalPowerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateMagicalPowerRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        MagicalPower result = request.ToMagicalPower();

        await magicalPowerService.CreateAsync(result, token);

        await outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.MagicalPowers, token);

        MagicalPowerResponse response = result.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    [HttpGet(ApiEndpoints.MagicalPowers.Get)]
    [OutputCache(PolicyName = ApiAssumptions.PolicyNames.MagicalPowers)]
    [ProducesResponseType<MagicalPowerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        MagicalPower? result = await magicalPowerService.GetByIdAsync(id, token);

        if (result is null)
        {
            return NotFound();
        }

        MagicalPowerResponse response = result.ToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.MagicalPowers.GetAll)]
    [OutputCache(PolicyName = ApiAssumptions.PolicyNames.MagicalPowers)]
    [ProducesResponseType<MagicalPowersResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(GetAllMagicalPowersRequest request, CancellationToken token)
    {
        GetAllMagicalPowersOptions options = request.ToOptions();

        IEnumerable<MagicalPower> results = await magicalPowerService.GetAllAsync(options, token);
        int total = await magicalPowerService.GetCountAsync(options, token);

        MagicalPowersResponse response = results.ToResponse(options.Page, options.PageSize, total);

        return Ok(response);
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPut(ApiEndpoints.MagicalPowers.Update)]
    [EnableRateLimiting("ThreeRequestsPerSecond")]
    [ProducesResponseType<MagicalPowerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(UpdateMagicalPowerRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        MagicalPower magicalPower = request.ToMagicalPower();

        bool result = await magicalPowerService.UpdateAsync(magicalPower, token);

        if (!result)
        {
            return NotFound();
        }

        MagicalPowerResponse response = magicalPower.ToResponse();

        await outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.MagicalPowers, token);
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.MagicalPowers.Delete)]
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

        bool result = await magicalPowerService.DeleteAsync(id, token);

        if (!result)
        {
            return NotFound();
        }

        await outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.MagicalPowers, token);
        return NoContent();
    }
}