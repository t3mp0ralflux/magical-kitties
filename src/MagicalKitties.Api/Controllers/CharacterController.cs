﻿using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Characters.Updates;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Contracts.Requests.Characters;
using MagicalKitties.Contracts.Responses.Characters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Attribute = MagicalKitties.Application.Models.Characters.Attribute;

namespace MagicalKitties.Api.Controllers;

[ApiController]
[Authorize]
public class CharacterController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICharacterService _characterService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CharacterController(IAccountService accountService, ICharacterService characterService, IDateTimeProvider dateTimeProvider)
    {
        _accountService = accountService;
        _characterService = characterService;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpPost(ApiEndpoints.Characters.Create)]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

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
                                  CreatedUtc = _dateTimeProvider.GetUtcNow(),
                                  UpdatedUtc = _dateTimeProvider.GetUtcNow(),
                                  Description = "",
                                  Hometown = "",
                                  Attributes =
                                  [
                                      new Attribute
                                      {
                                          Id = Guid.NewGuid(),
                                          Name = "Cute",
                                          Value = 0
                                      },
                                      new Attribute
                                      {
                                          Id = Guid.NewGuid(),
                                          Name = "Cunning",
                                          Value = 0
                                      },
                                      new Attribute
                                      {
                                          Id = Guid.NewGuid(),
                                          Name = "Fierce",
                                          Value = 0
                                      }
                                  ],
                                  Level = 1,
                                  CurrentXp = 0,
                                  MaxOwies = 2,
                                  StartingTreats = 2
                              };

        await _characterService.CreateAsync(character, token);

        CharacterResponse response = character.ToResponse();

        return CreatedAtAction(nameof(Get), new { id = character.Id }, response);
    }

    [HttpGet(ApiEndpoints.Characters.Get)]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail()!, token);

        if (account is null)
        {
            return Unauthorized();
        }

        Character? character = await _characterService.GetByIdAsync(id, token);

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
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail()!, token);

        if (account is null)
        {
            return Unauthorized();
        }

        GetAllCharactersOptions options = request.ToOptions(account.Id);

        IEnumerable<Character> characters = await _characterService.GetAllAsync(options, token);
        int characterCount = await _characterService.GetCountAsync(options, token);

        CharactersResponse response = characters.ToGetAllResponse(request.Page, request.PageSize, characterCount);

        return Ok(response);
    }

    [HttpPut(ApiEndpoints.Characters.Update)]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] CharacterUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail()!, token);

        if (account is null)
        {
            return Unauthorized();
        }

        Character character = request.ToCharacter(account);

        bool result = await _characterService.UpdateAsync(character, token);

        if (!result)
        {
            return NotFound();
        }

        CharacterResponse response = character.ToResponse();

        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Characters.Delete)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool result = await _characterService.DeleteAsync(id, token);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut(ApiEndpoints.Characters.ChangeLevel)]
    [ProducesResponseType<CharacterUpdateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeLevel([FromBody] CharacterLevelUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        LevelUpdate update = request.ToUpdate();

        // will throw validationexception on errors.
        bool success = await _characterService.UpdateLevelAsync(update, token);

        if (!success)
        {
            return NotFound();
        }

        CharacterUpdateResponse result = update.ToResponse("Level update was successful");

        return Ok(result);
    }

    [HttpPut(ApiEndpoints.Characters.ChangeFlaw)]
    [ProducesResponseType<CharacterUpdateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeFlaw([FromBody] CharacterFlawUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        FlawUpdate update = request.ToUpdate();

        bool success = await _characterService.UpdateFlawAsync(update, token);

        if (!success)
        {
            return NotFound();
        }

        CharacterUpdateResponse result = update.ToResponse("Flaw update was successful");

        return Ok(result);
    }

    [HttpPut(ApiEndpoints.Characters.ChangeTalent)]
    [ProducesResponseType<CharacterUpdateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<BadRequestResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeTalent([FromBody] CharacterTalentUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        TalentUpdate update = request.ToUpdate();

        bool success = await _characterService.UpdateTalentAsync(update, token);

        if (!success)
        {
            return NotFound();
        }

        CharacterUpdateResponse result = update.ToResponse("Flaw update was successful");

        return Ok(result);
    }
}