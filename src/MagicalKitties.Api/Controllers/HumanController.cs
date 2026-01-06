using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Responses.Errors;
using MagicalKitties.Contracts.Responses.Humans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKCtrHumanRequests = MagicalKitties.Contracts.Requests.Humans;

namespace MagicalKitties.Api.Controllers;

[ApiController]
[Authorize]
public class HumanController(IHumanService humanService, IAccountService accountService, ICharacterService characterService) : ControllerBase
{
    [HttpPost(ApiEndpoints.Humans.Create)]
    [ProducesResponseType<HumanResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<NotFoundObjectResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(Guid characterId, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }
        
        bool? characterExists = await characterService.ExistsByIdAsync(account.Id, characterId, token);

        if (!characterExists.HasValue)
        {
            return Forbid();
        }

        if (!characterExists.Value)
        {
            return NotFound("Character not found.");
        }
        
        Human human = await humanService.CreateAsync(characterId, token);

        HumanResponse response = human.ToResponse();

        return CreatedAtAction(nameof(Get), new { characterId, humanId = human.Id }, response);
    }

    [HttpPost(ApiEndpoints.Humans.CreateProblem)]
    [ProducesResponseType<ProblemResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateProblem([FromRoute] Guid characterId, [FromRoute] Guid humanId, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Problem result = await humanService.CreateProblemAsync(characterId, humanId, token);

        ProblemResponse response = result.ToResponse();

        return Created(string.Empty, response);
    }

    [HttpGet(ApiEndpoints.Humans.Get)]
    [ProducesResponseType<HumanResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid characterId, [FromRoute] Guid humanId, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool? characterExists = await characterService.ExistsByIdAsync(account.Id, characterId, token);

        if (!characterExists.HasValue)
        {
            return Forbid();
        }

        if (!characterExists.Value)
        {
            return NotFound("Character not found.");
        }

        Human? human = await humanService.GetByIdAsync(characterId, humanId, token);

        if (human is null)
        {
            return NotFound();
        }

        HumanResponse response = human.ToResponse();

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Humans.GetAll)]
    [ProducesResponseType<HumansResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(MKCtrHumanRequests.GetAllHumansRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        GetAllHumansOptions options = request.ToOptions();

        IEnumerable<Human> result = await humanService.GetAllAsync(options, token);
        int humanCount = await humanService.GetCountAsync(options, token);

        HumansResponse response = result.ToGetAllResponse(request.Page, request.PageSize, humanCount);

        return Ok(response);
    }

    [HttpPut(ApiEndpoints.Humans.Update)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHuman([FromRoute] MKCtrHumanRequests.DescriptionOption description, [FromBody] MKCtrHumanRequests.HumanDescriptionUpdateRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        DescriptionUpdate descriptionUpdate = request.ToUpdate(description);

        bool success = await humanService.UpdateDescriptionAsync(descriptionUpdate, token);

        return success ? Ok($"{description.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpPut(ApiEndpoints.Humans.UpdateProblem)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProblem([FromRoute] MKCtrHumanRequests.ProblemOption problemOption, [FromBody] MKCtrHumanRequests.HumanProblemUpdateRequest request, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        ProblemUpdate problemUpdate = request.ToUpdate(problemOption);

        bool success = await humanService.UpdateProblemAsync(problemUpdate, token);

        return success ? Ok($"{problemOption.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpDelete(ApiEndpoints.Humans.Delete)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid characterId,[FromRoute] Guid humanId, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool? characterExists = await characterService.ExistsByIdAsync(account.Id, characterId, token);

        if (!characterExists.HasValue)
        {
            return Forbid();
        }

        if (!characterExists.Value)
        {
            return NotFound("Character not found.");
        }

        bool result = await humanService.DeleteAsync(characterId, humanId, token);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete(ApiEndpoints.Humans.DeleteProblem)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProblem([FromRoute] Guid characterId,[FromRoute] Guid humanId, [FromRoute] Guid problemId, CancellationToken token)
    {
        Account? account = await accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool result = await humanService.DeleteProblemAsync(characterId, humanId, problemId, token);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}