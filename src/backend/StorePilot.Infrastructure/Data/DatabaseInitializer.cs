using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StorePilot.Domain.Entities;

namespace StorePilot.Infrastructure.Data;

public sealed class DatabaseInitializer(
    StorePilotDbContext dbContext,
    ILogger<DatabaseInitializer> logger)
{
    private static readonly string[] DefaultUnitNames =
    [
        "متر",
        "قطعة"
    ];

    private static readonly string[] DefaultGroupNames =
    [
        "نواكل",
        "قطع بولي",
        "قطع ابيض",
        "مواسير بولي",
        "مواسير ابيض",
        "اطقم صيني",
        "نواكل صيني",
        "قطع ابيض صيني",
        "حدايد وبويات",
        "كهرباء",
        "موبايل قسط",
        "أجهزة كهربائية قسط",
        "مستلزمات قسط"
    ];

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting database initialization.");

        await dbContext.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Database migrations applied successfully.");

        var canConnect = await dbContext.Database.CanConnectAsync(
            cancellationToken);

        if (!canConnect)
        {
            throw new InvalidOperationException(
                "Unable to connect to the StorePilot database.");
        }

        logger.LogInformation("Database connection verified successfully.");

        await SeedReferenceDataAsync(cancellationToken);
    }

    private async Task SeedReferenceDataAsync(
        CancellationToken cancellationToken)
    {
        var existingUnitNames = await dbContext.Units
            .Select(unit => unit.Name)
            .ToListAsync(cancellationToken);

        var unitNames = existingUnitNames.ToHashSet(
            StringComparer.Ordinal);

        var unitsToAdd = DefaultUnitNames
            .Where(unitNames.Add)
            .Select(name => new Unit { Name = name })
            .ToArray();

        var existingGroupNames = await dbContext.ProductGroups
            .Select(group => group.Name)
            .ToListAsync(cancellationToken);

        var groupNames = existingGroupNames.ToHashSet(
            StringComparer.Ordinal);

        var groupsToAdd = DefaultGroupNames
            .Where(groupNames.Add)
            .Select(name => new ProductGroup { Name = name })
            .ToArray();

        if (unitsToAdd.Length == 0 && groupsToAdd.Length == 0)
        {
            logger.LogInformation(
                "Reference data is already initialized.");

            return;
        }

        dbContext.Units.AddRange(unitsToAdd);
        dbContext.ProductGroups.AddRange(groupsToAdd);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Reference data initialized. Added {UnitCount} units and {GroupCount} groups.",
            unitsToAdd.Length,
            groupsToAdd.Length);
    }
}
