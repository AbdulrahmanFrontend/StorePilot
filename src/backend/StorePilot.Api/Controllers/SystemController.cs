using Microsoft.AspNetCore.Mvc;
using StorePilot.Application.Interfaces;
using StorePilot.Infrastructure.Data;

namespace StorePilot.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController(
    StorePilotDbContext dbContext,
    IStoreSettingsService settingsService) : ControllerBase
{
    [HttpGet("health")]
    public async Task<IActionResult> Health(CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new { status = "Unavailable" });
        }

        return Ok(new { status = "Ready" });
    }

    // GET: api/system/setup-status
    [HttpGet("setup-status")]
    public async Task<IActionResult> GetSetupStatus(CancellationToken cancellationToken)
    {
        var isSetupCompleted = await settingsService.IsSetupCompletedAsync(
            cancellationToken);

        return Ok(new
        {
            isSetupCompleted
        });
    }
}
