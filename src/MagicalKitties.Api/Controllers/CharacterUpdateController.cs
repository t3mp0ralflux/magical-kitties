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
public class CharacterUpdateController(IAccountService accountService, ICharacterService characterService, ICharacterUpdateService characterUpdateService, ICharacterUpgradeService characterUpgradeService) : ControllerBase
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

        Character? character = await characterService.GetByIdAsync(request.CharacterId, token);

        if (character is null)
        {
            return NotFound();
        }

        if (character.AccountId != account.Id)
        {
            return Unauthorized();
        }

        MKCtrApplicationCharacterUpdates.DescriptionUpdate descriptionUpdate = request.ToUpdate(account.Id);

        // will throw validation errors
        await characterUpdateService.UpdateDescriptionAsync((MKCtrApplicationCharacterUpdates.DescriptionOption)description, descriptionUpdate, token);
        
        return Ok($"{description.ToString()} updated successfully.");
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

        Character? character = await characterService.GetByIdAsync(request.CharacterId, token);

        if (character is null)
        {
            return NotFound();
        }

        if (character.AccountId != account.Id)
        {
            return Unauthorized();
        }

        MKCtrApplicationCharacterUpdates.AttributeUpdate attributeUpdate = request.ToUpdate(character);

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

        Character? character = await characterService.GetByIdAsync(id, token);

        if (character is null)
        {
            return NotFound();
        }

        if (character.AccountId != account.Id)
        {
            return Unauthorized();
        }

        await characterUpdateService.Reset(id, token);

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

        Character? character = await characterService.GetByIdAsync(characterId, token);

        if (character is null)
        {
            return NotFound();
        }

        if (character.AccountId != account.Id)
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

        Character? character = await characterService.GetByIdAsync(characterId, token);
        
        if (character is null)
        {
            return NotFound();
        }

        if (character.AccountId != account.Id)
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