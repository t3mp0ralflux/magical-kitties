using FluentValidation.Results;
using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Characters;
using MagicalKitties.Contracts.Responses.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKCtrCharacterRequests = MagicalKitties.Contracts.Requests.Characters;

namespace MagicalKitties.Api.Controllers;

[Authorize]
[ApiController]
public class CharacterUpdateController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICharacterUpdateService _characterUpdateService;

    public CharacterUpdateController(IAccountService accountService, ICharacterUpdateService characterUpdateService)
    {
        _accountService = accountService;
        _characterUpdateService = characterUpdateService;
    }

    [HttpPut(ApiEndpoints.Characters.UpdateDescription)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundObjectResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDescription([FromRoute]MKCtrCharacterRequests.DescriptionOptions description, [FromBody]CharacterDescriptionUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        DescriptionUpdate descriptionUpdate = request.ToUpdate(account.Id, description);

        // will throw validation errors
        bool success = await _characterUpdateService.UpdateDescriptionAsync(descriptionUpdate, token);

        // returns not found if character not found
        return success ? Ok($"{description.ToString()} updated successfully.") : NotFound("Character not found.");
    }
}