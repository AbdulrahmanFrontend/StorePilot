using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorePilot.Infrastructure.Data;

namespace StorePilot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databasePath)
    {
        var connectionString =
            $"Data Source={databasePath};Default Timeout=5";

        services.AddDbContext<StorePilotDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        return services;
    }
}
