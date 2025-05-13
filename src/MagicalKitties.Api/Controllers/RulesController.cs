using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.Rules;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Responses.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class RulesController : ControllerBase
{
    private readonly IRuleService _ruleService;

    public RulesController(IRuleService ruleService)
    {
        _ruleService = ruleService;
    }

    [HttpGet(ApiEndpoints.Rules.GetAll)]
    [OutputCache(PolicyName = ApiAssumptions.PolicyNames.Rules)]
    [ProducesResponseType<GameRulesResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRules(CancellationToken token)
    {
        GameRules rules = await _ruleService.GetAll(token);

        GameRulesResponse response = rules.ToResponse();

        return Ok(response);
    }
}