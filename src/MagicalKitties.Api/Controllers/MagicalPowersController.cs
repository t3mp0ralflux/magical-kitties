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

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class MagicalPowersController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMagicalPowerService _magicalPowerService;
    private readonly IOutputCacheStore _outputCacheStore;

    public MagicalPowersController(IAccountService accountService, IMagicalPowerService magicalPowerService, IOutputCacheStore outputCacheStore)
    {
        _accountService = accountService;
        _magicalPowerService = magicalPowerService;
        _outputCacheStore = outputCacheStore;
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPost(ApiEndpoints.MagicalPowers.Create)]
    [ProducesResponseType<MagicalPowerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateMagicalPowerRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        MagicalPower result = request.ToMagicalPower();

        await _magicalPowerService.CreateAsync(result, token);
        
        await _outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.MagicalPowers, token);

        MagicalPowerResponse response = result.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    [HttpGet(ApiEndpoints.MagicalPowers.Get)]
    [OutputCache(PolicyName = ApiAssumptions.PolicyNames.MagicalPowers)]
    [ProducesResponseType<MagicalPowerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        MagicalPower? result = await _magicalPowerService.GetByIdAsync(id, token);

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

        IEnumerable<MagicalPower> results = await _magicalPowerService.GetAllAsync(options, token);
        int total = await _magicalPowerService.GetCountAsync(options, token);

        MagicalPowersResponse response = results.ToResponse(options.Page, options.PageSize, total);

        return Ok(response);
    }

    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPut(ApiEndpoints.MagicalPowers.Update)]
    [ProducesResponseType<MagicalPowerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(UpdateMagicalPowerRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        MagicalPower magicalPower = request.ToMagicalPower();

        bool result = await _magicalPowerService.UpdateAsync(magicalPower, token);

        if (!result)
        {
            return NotFound();
        }

        MagicalPowerResponse response = magicalPower.ToResponse();

        await _outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.MagicalPowers, token);
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.MagicalPowers.Delete)]
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

        bool result = await _magicalPowerService.DeleteAsync(id, token);

        if (!result)
        {
            return NotFound();
        }

        await _outputCacheStore.EvictByTagAsync(ApiAssumptions.TagNames.MagicalPowers, token);
        return NoContent();
    }
}