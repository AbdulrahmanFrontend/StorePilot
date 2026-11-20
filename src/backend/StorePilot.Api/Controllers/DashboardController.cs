using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorePilot.Application.Dtos.Dashboard;
using StorePilot.Infrastructure.Data;

namespace StorePilot.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(
    StorePilotDbContext dbContext) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        CancellationToken cancellationToken)
    {
        var summary = new DashboardSummaryDto(
            ActiveProductsCount: await dbContext.Products.CountAsync(
                product => product.IsActive,
                cancellationToken),

            ActiveGroupsCount: await dbContext.ProductGroups.CountAsync(
                group => group.IsActive,
                cancellationToken),

            ActiveUnitsCount: await dbContext.Units.CountAsync(
                unit => unit.IsActive,
                cancellationToken)
        );

        return Ok(summary);
    }
}
