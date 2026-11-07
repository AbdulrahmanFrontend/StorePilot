using Microsoft.AspNetCore.Mvc;
using StorePilot.Infrastructure.Data;

namespace StorePilot.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController(
    StorePilotDbContext dbContext) : ControllerBase
{
    [HttpGet("health")]
    public async Task<IActionResult> Health(
        CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database
            .CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new { status = "Unavailable" });
        }

        return Ok(new { status = "Ready" });
    }
}
