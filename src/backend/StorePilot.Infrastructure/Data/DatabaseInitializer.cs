using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StorePilot.Infrastructure.Data;

public sealed class DatabaseInitializer(
    StorePilotDbContext dbContext,
    ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting database initialization.");

        await dbContext.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Database migrations applied successfully.");

        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
            throw new InvalidOperationException(
                "Unable to connect to the StorePilot database.");
        }

        logger.LogInformation("Database connection verified successfully.");
    }
}
