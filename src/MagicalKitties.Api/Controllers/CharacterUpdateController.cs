using FluentValidation;
using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Characters;
using MagicalKitties.Contracts.Responses.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKCtrApplicationCharacterUpdates = MagicalKitties.Application.Models.Characters.Updates;
using MKCtrCharacterRequests = MagicalKitties.Contracts.Requests.Characters;

namespace MagicalKitties.Api.Controllers;

[Authorize]
[ApiController]
public class CharacterUpdateController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICharacterUpdateService _characterUpdateService;
    private readonly ICharacterUpgradeService _characterUpgradeService;

    public CharacterUpdateController(IAccountService accountService, ICharacterUpdateService characterUpdateService, ICharacterUpgradeService characterUpgradeService)
    {
        _accountService = accountService;
        _characterUpdateService = characterUpdateService;
        _characterUpgradeService = characterUpgradeService;
    }

    [HttpPut(ApiEndpoints.Characters.UpdateDescription)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundObjectResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDescription([FromRoute]MKCtrCharacterRequests.DescriptionOption description, [FromBody]CharacterDescriptionUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        DescriptionUpdate descriptionUpdate = request.ToUpdate(account.Id);

        // will throw validation errors
        bool success = await _characterUpdateService.UpdateDescriptionAsync((MKCtrApplicationCharacterUpdates.DescriptionOption)description, descriptionUpdate, token);

        // returns not found if character not found
        return success ? Ok($"{description.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpPut(ApiEndpoints.Characters.UpdateAttribute)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundObjectResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAttribute([FromRoute]MKCtrCharacterRequests.AttributeOption attribute, [FromBody]CharacterAttributeUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        AttributeUpdate attributeUpdate = request.ToUpdate(account.Id);
        
        // will throw validation errors
        bool success = await _characterUpdateService.UpdateAttributeAsync((MKCtrApplicationCharacterUpdates.AttributeOption)attribute, attributeUpdate, token);

        return success ? Ok($"{attribute.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpPut(ApiEndpoints.Characters.Reset)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reset([FromRoute] Guid id, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool success = await _characterUpdateService.Reset(account.Id, id, token);

        if (!success)
        {
            return NotFound();
        }

        return Ok("Character reset successfully.");
    }

    [HttpPut(ApiEndpoints.Characters.UpsertUpgrade)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationException>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpsertUpgrade([FromRoute]Guid characterId, UpgradeUpsertRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        UpgradeRequest update = request.ToUpdate(account.Id, characterId);

        bool success = await _characterUpgradeService.UpsertUpgradeAsync(update, token);

        if (!success)
        {
            return NotFound();
        }

        return Ok("Upgrade update was successful.");
    }

    [HttpPut(ApiEndpoints.Characters.RemoveUpgrade)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationException>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUpgrade([FromRoute]Guid characterId, UpgradeRemoveRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        UpgradeRequest update = request.ToUpdate(account.Id, characterId);

        bool success = await _characterUpgradeService.RemoveUpgradeAsync(update, token);
        
        if (!success)
        {
            return NotFound();
        }

        return Ok("Upgrade update was successful.");
    }
}