using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.GlobalSetting;
using MagicalKitties.Contracts.Responses.GlobalSetting;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class GlobalSettingController : ControllerBase
{
    private readonly IGlobalSettingsService _globalSettingsService;

    public GlobalSettingController(IGlobalSettingsService globalSettingsService)
    {
        _globalSettingsService = globalSettingsService;
    }

    [HttpPost(ApiEndpoints.GlobalSettings.Create)]
    public async Task<IActionResult> Create([FromBody] GlobalSettingCreateRequest createRequest, CancellationToken token)
    {
        GlobalSetting setting = createRequest.ToGlobalSetting();

        await _globalSettingsService.CreateSettingAsync(setting, token);

        GlobalSettingResponse response = setting.ToResponse();

        return CreatedAtAction(nameof(Get), new { response.Id }, response);
    }

    [HttpGet(ApiEndpoints.GlobalSettings.Get)]
    public async Task<IActionResult> Get([FromRoute] string name, CancellationToken token)
    {
        GlobalSetting? setting = await _globalSettingsService.GetSettingAsync(name, token);

        return Ok(setting);
    }

    [HttpGet(ApiEndpoints.GlobalSettings.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllGlobalSettingsRequest request, CancellationToken token)
    {
        GetAllGlobalSettingsOptions options = request.ToOptions();
        IEnumerable<GlobalSetting> result = await _globalSettingsService.GetAllAsync(options, token);
        int settingCount = await _globalSettingsService.GetCountAsync(options.Name!, token);

        GlobalSettingsResponse response = result.ToResponse(request.Page, request.PageSize, settingCount);

        return Ok(response);
    }
}