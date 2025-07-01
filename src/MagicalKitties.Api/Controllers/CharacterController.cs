using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Characters;
using MagicalKitties.Contracts.Responses.Characters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiController]
[Authorize]
public class CharacterController(IAccountService accountService, ICharacterService characterService, IDateTimeProvider dateTimeProvider) : ControllerBase
{
    [HttpPost(ApiEndpoints.Characters.Create)]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Character character = new()
                              {
                                  Id = Guid.NewGuid(),
                                  AccountId = account.Id,
                                  Username = account.Username,
                                  Name = $"{account.Username}'s Unnamed Character",
                                  CreatedUtc = dateTimeProvider.GetUtcNow(),
                                  UpdatedUtc = dateTimeProvider.GetUtcNow(),
                                  Description = "",
                                  Hometown = "",
                                  Cunning = 0,
                                  Cute = 0,
                                  Fierce = 0,
                                  Level = 1,
                                  CurrentXp = 0,
                                  MaxOwies = 2,
                                  StartingTreats = 2
                              };

        await characterService.CreateAsync(character, token);

        CharacterResponse response = character.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = character.Id }, response);
    }

    [HttpPost(ApiEndpoints.Characters.Copy)]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Copy([FromRoute] Guid id, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool characterExists = await characterService.ExistsByIdAsync(id, token);

        if (!characterExists)
        {
            return NotFound();
        }

        Character characterCopy = await characterService.CopyAsync(account, id, token);
        
        CharacterResponse response = characterCopy.ToResponse();
        
        return CreatedAtAction(nameof(Get), new { id = characterCopy.Id }, response);
    }

    [HttpGet(ApiEndpoints.Characters.Get)]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail()!, token);

        if (account is null)
        {
            return Unauthorized();
        }

        Character? character = await characterService.GetByIdAsync(account.Id, id, token);

        if (character is null)
        {
            return NotFound();
        }

        CharacterResponse response = character.ToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Characters.GetAll)]
    [ProducesResponseType<CharactersResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllCharactersRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail()!, token);

        if (account is null)
        {
            return Unauthorized();
        }

        GetAllCharactersOptions options = request.ToOptions(account.Id);

        IEnumerable<Character> characters = await characterService.GetAllAsync(options, token);
        int characterCount = await characterService.GetCountAsync(options, token);

        CharactersResponse response = characters.ToGetAllResponse(request.Page, request.PageSize, characterCount);

        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Characters.Delete)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        bool accountExists = await accountService.ExistsByEmailAsync(HttpContext.GetUserEmail(), token);

        if (!accountExists)
        {
            return Unauthorized();
        }

        bool result = await characterService.DeleteAsync(id, token);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}