using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.GlobalSetting;
using MagicalKitties.Contracts.Responses.GlobalSetting;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiController]
public class GlobalSettingController(IGlobalSettingsService globalSettingsService) : ControllerBase
{
    [HttpPost(ApiEndpoints.GlobalSettings.Create)]
    public async Task<IActionResult> Create([FromBody] GlobalSettingCreateRequest createRequest, CancellationToken token)
    {
        GlobalSetting setting = createRequest.ToGlobalSetting();

        await globalSettingsService.CreateSettingAsync(setting, token);

        GlobalSettingResponse response = setting.ToResponse();

        return CreatedAtAction(nameof(Get), new { response.Id }, response);
    }

    [HttpGet(ApiEndpoints.GlobalSettings.Get)]
    public async Task<IActionResult> Get([FromRoute] string name, CancellationToken token)
    {
        GlobalSetting? setting = await globalSettingsService.GetSettingAsync(name, token);

        return Ok(setting);
    }

    [HttpGet(ApiEndpoints.GlobalSettings.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllGlobalSettingsRequest request, CancellationToken token)
    {
        GetAllGlobalSettingsOptions options = request.ToOptions();
        IEnumerable<GlobalSetting> result = await globalSettingsService.GetAllAsync(options, token);
        int settingCount = await globalSettingsService.GetCountAsync(options.Name!, token);

        GlobalSettingsResponse response = result.ToResponse(request.Page, request.PageSize, settingCount);

        return Ok(response);
    }
}