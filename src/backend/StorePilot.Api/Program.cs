using Serilog;
using StorePilot.Api.ExceptionHandling;
using StorePilot.Infrastructure;
using StorePilot.Infrastructure.Data;
using StorePilot.Infrastructure.Storage;

try
{
    var appDataPaths = new AppDataPaths();

    Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(appDataPaths.LogsDirectory, "storepilot-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} " +
            "[{Level:u3}] {SourceContext} " +
            "{Message:lj}{NewLine}{Exception}")
    .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Host.UseSerilog();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddSingleton(appDataPaths);

    builder.Services.AddInfrastructure(
        appDataPaths.DatabasePath);

    builder.Services.AddScoped<DatabaseInitializer>();

    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseExceptionHandler();

    app.Logger.LogInformation(
        "StorePilot application data directory: {RootPath}",
        appDataPaths.RootPath);

    await using (var scope = app.Services.CreateAsyncScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        await initializer.InitializeAsync();
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpMethodOverride();

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "StorePilot terminated unexpectedly.");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
