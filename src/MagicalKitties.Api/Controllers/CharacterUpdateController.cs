using FluentValidation;
using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Responses.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKCtrApplicationCharacterUpdates = MagicalKitties.Application.Models.Characters.Updates;
using MKCtrCharacterRequests = MagicalKitties.Contracts.Requests.Characters;

namespace MagicalKitties.Api.Controllers;

[Authorize]
[ApiController]
public class CharacterUpdateController(IAccountService accountService, ICharacterUpdateService characterUpdateService, ICharacterUpgradeService characterUpgradeService) : ControllerBase
{
    [HttpPut(ApiEndpoints.Characters.UpdateDescription)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundObjectResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDescription([FromRoute] MKCtrCharacterRequests.DescriptionOption description, [FromBody] MKCtrCharacterRequests.CharacterDescriptionUpdateRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        MKCtrApplicationCharacterUpdates.DescriptionUpdate descriptionUpdate = request.ToUpdate(account.Id);

        // will throw validation errors
        bool success = await characterUpdateService.UpdateDescriptionAsync((MKCtrApplicationCharacterUpdates.DescriptionOption)description, descriptionUpdate, token);

        // returns not found if character not found
        return success ? Ok($"{description.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpPut(ApiEndpoints.Characters.UpdateAttribute)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundObjectResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAttribute([FromRoute] MKCtrCharacterRequests.AttributeOption attribute, [FromBody] MKCtrCharacterRequests.CharacterAttributeUpdateRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        MKCtrApplicationCharacterUpdates.AttributeUpdate attributeUpdate = request.ToUpdate(account.Id);

        // will throw validation errors
        bool success = await characterUpdateService.UpdateAttributeAsync((MKCtrApplicationCharacterUpdates.AttributeOption)attribute, attributeUpdate, token);

        return success ? Ok($"{attribute.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpPut(ApiEndpoints.Characters.Reset)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reset([FromRoute] Guid id, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool success = await characterUpdateService.Reset(account.Id, id, token);

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
    public async Task<IActionResult> UpsertUpgrade([FromRoute] Guid characterId, MKCtrCharacterRequests.UpgradeUpsertRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        UpgradeRequest update = request.ToUpdate(account.Id, characterId);

        bool success = await characterUpgradeService.UpsertUpgradeAsync(update, token);

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
    public async Task<IActionResult> RemoveUpgrade([FromRoute] Guid characterId, MKCtrCharacterRequests.UpgradeRemoveRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        UpgradeRequest update = request.ToUpdate(account.Id, characterId);

        bool success = await characterUpgradeService.RemoveUpgradeAsync(update, token);

        if (!success)
        {
            return NotFound();
        }

        return Ok("Upgrade update was successful.");
    }
}