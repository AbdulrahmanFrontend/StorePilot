using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorePilot.Application.Dtos.ReferenceData;
using StorePilot.Infrastructure.Data;

namespace StorePilot.Api.Controllers;

[ApiController]
[Route("api/groups")]
public sealed class GroupsController(
    StorePilotDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReferenceDataItemDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var groups = await dbContext.ProductGroups
            .AsNoTracking()
            .OrderBy(group => group.Name)
            .Select(group => new ReferenceDataItemDto(
                group.Id,
                group.Name))
            .ToListAsync(cancellationToken);

        return Ok(groups);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReferenceDataItemDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var group = await dbContext.ProductGroups
            .AsNoTracking()
            .Where(group => group.Id == id)
            .Select(group => new ReferenceDataItemDto(
                group.Id,
                group.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (group is null)
        {
            return NotFound();
        }

        return Ok(group);
    }
}
