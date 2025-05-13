using MagicalKitties.Application.HostedServices;
using Microsoft.AspNetCore.Mvc;

namespace MagicalKitties.Api.Controllers;

[ApiController]
[Route("services")]
public class ServicesController : ControllerBase
{
    private readonly IServiceProvider _services;

    public ServicesController(IServiceProvider services)
    {
        _services = services;
    }

    [HttpPost("stop/{name}")]
    public async Task<IActionResult> StopService([FromRoute] string name, CancellationToken token)

    {
        switch (name.ToLowerInvariant())
        {
            case "email":
                EmailProcessingService emailProcessor = _services.GetRequiredService<EmailProcessingService>();
                await emailProcessor.StopAsync(token);
                break;
            default:
                return NotFound();
        }

        return Ok($"{name} service stopped.");
    }

    [HttpPost("start/{name}")]
    public async Task<IActionResult> StartService([FromRoute] string name, CancellationToken token)
    {
        switch (name.ToLowerInvariant())
        {
            case "email":
                EmailProcessingService emailProcessor = _services.GetRequiredService<EmailProcessingService>();
                await emailProcessor.StartAsync(token);
                break;
            default:
                return NotFound();
        }

        return Ok($"{name} service started.");
    }
}