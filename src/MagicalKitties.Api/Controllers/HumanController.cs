using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.Humans;
using MagicalKitties.Contracts.Responses.Errors;
using MagicalKitties.Contracts.Responses.Humans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MKCtrHumanRequests = MagicalKitties.Contracts.Requests.Humans;

namespace MagicalKitties.Api.Controllers;

[ApiController]
[Authorize]
public class HumanController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICharacterService _characterService;
    private readonly IHumanService _humanService;
    
    public HumanController(IHumanService humanService, IAccountService accountService, ICharacterService characterService)
    {
        _humanService = humanService;
        _accountService = accountService;
        _characterService = characterService;
    }

    [HttpPost(ApiEndpoints.Humans.Create)]
    [ProducesResponseType<HumanResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<NotFoundObjectResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(Guid characterId, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool characterExists = await _characterService.ExistsByIdAsync(characterId, token);

        if (!characterExists)
        {
            return NotFound("Character not found.");
        }
        
        Human human = await _humanService.CreateAsync(characterId, token);

        HumanResponse response = human.ToResponse();

        return CreatedAtAction(nameof(Get), characterId, new { response });

    }

    [HttpPost(ApiEndpoints.Humans.CreateProblem)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateProblem([FromRoute]Guid humanId, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }
        
        bool result = await _humanService.CreateProblemAsync(humanId, token);

        return result ? Ok("Problem created successfully") : NotFound("Human not found.");
    }

    [HttpGet(ApiEndpoints.Humans.Get)]
    [ProducesResponseType<HumanResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        Human? human = await _humanService.GetByIdAsync(id, token);

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
    public async Task<IActionResult> GetAll(GetAllHumansRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        GetAllHumansOptions options = request.ToOptions();

        IEnumerable<Human> result = await _humanService.GetAllAsync(options, token);
        int humanCount = await _humanService.GetCountAsync(options, token);

        HumansResponse response = result.ToGetAllResponse(request.Page, request.PageSize, humanCount);

        return Ok(response);
    }

    [HttpPut(ApiEndpoints.Humans.Update)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHuman([FromRoute]MKCtrHumanRequests.DescriptionOption description, [FromBody]HumanDescriptionUpdateRequest request, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        DescriptionUpdate descriptionUpdate = request.ToUpdate(description);

        bool success = await _humanService.UpdateDescriptionAsync(descriptionUpdate, token);

        return success ? Ok($"{description.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpPut(ApiEndpoints.Humans.UpdateProblem)]
    [ProducesResponseType<OkObjectResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ValidationFailureResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProblem([FromRoute]MKCtrHumanRequests.ProblemOption problem, [FromBody]HumanProblemUpdateRequest request, CancellationToken token )
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        ProblemUpdate problemUpdate = request.ToUpdate(problem);
        
        bool success = await _humanService.UpdateProblemAsync(problemUpdate, token);

        return success ? Ok($"{problem.ToString()} updated successfully.") : NotFound("Character not found.");
    }

    [HttpDelete(ApiEndpoints.Humans.Delete)]
    [ProducesResponseType<NoContentResult>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<NotFoundResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool result = await _humanService.DeleteAsync(id, token);

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
    public async Task<IActionResult> DeleteProblem([FromRoute]Guid humanId, [FromRoute]Guid problemId, CancellationToken token)
    {
        Account? account = await _accountService.GetByEmailAsync(HttpContext.GetUserEmail(), token);

        if (account is null)
        {
            return Unauthorized();
        }

        bool result = await _humanService.DeleteProblemAsync(humanId, problemId, token);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    
}