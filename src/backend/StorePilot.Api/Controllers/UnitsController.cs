using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorePilot.Application.Dtos.ReferenceData;
using StorePilot.Infrastructure.Data;

namespace StorePilot.Api.Controllers;

[ApiController]
[Route("api/units")]
public sealed class UnitsController(
    StorePilotDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReferenceDataItemDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var units = await dbContext.Units
            .AsNoTracking()
            .OrderBy(unit => unit.Name)
            .Select(unit => new ReferenceDataItemDto(
                unit.Id,
                unit.Name))
            .ToListAsync(cancellationToken);

        return Ok(units);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReferenceDataItemDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var unit = await dbContext.Units
            .AsNoTracking()
            .Where(unit => unit.Id == id)
            .Select(unit => new ReferenceDataItemDto(
                unit.Id,
                unit.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (unit is null)
        {
            return NotFound();
        }

        return Ok(unit);
    }
}
