using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using StorePilot.Domain.Entities;
using StorePilot.Infrastructure.Data;

namespace StorePilot.IntegrationTests.ReferenceData;

public sealed class DatabaseInitializerTests
{
    private static readonly string[] ExpectedUnits =
    [
        "متر",
        "قطعة"
    ];

    private static readonly string[] ExpectedGroups =
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

    [Fact]
    public async Task InitializeAsync_WhenDatabaseIsEmpty_SeedsDefaultReferenceData()
    {
        // Arrange
        await using var connection = new SqliteConnection(
            "Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<StorePilotDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new StorePilotDbContext(options);

        var sut = CreateInitializer(dbContext);

        // Act
        await sut.InitializeAsync();

        // Assert
        var actualUnits = await dbContext.Units
            .Select(unit => unit.Name)
            .ToListAsync();

        var actualGroups = await dbContext.ProductGroups
            .Select(group => group.Name)
            .ToListAsync();

        Assert.Equal(
            ExpectedUnits.OrderBy(name => name),
            actualUnits.OrderBy(name => name));

        Assert.Equal(
            ExpectedGroups.OrderBy(name => name),
            actualGroups.OrderBy(name => name));

        Assert.Equal(2, actualUnits.Count);
        Assert.Equal(13, actualGroups.Count);
    }

    [Fact]
    public async Task InitializeAsync_WhenCalledTwice_DoesNotCreateDuplicates()
    {
        // Arrange
        await using var connection = new SqliteConnection(
            "Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<StorePilotDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new StorePilotDbContext(options);

        var sut = CreateInitializer(dbContext);

        // Act
        await sut.InitializeAsync();

        var firstUnitIds = await dbContext.Units
            .OrderBy(unit => unit.Id)
            .Select(unit => unit.Id)
            .ToListAsync();

        var firstGroupIds = await dbContext.ProductGroups
            .OrderBy(group => group.Id)
            .Select(group => group.Id)
            .ToListAsync();

        await sut.InitializeAsync();

        var secondUnitIds = await dbContext.Units
            .OrderBy(unit => unit.Id)
            .Select(unit => unit.Id)
            .ToListAsync();

        var secondGroupIds = await dbContext.ProductGroups
            .OrderBy(group => group.Id)
            .Select(group => group.Id)
            .ToListAsync();

        // Assert
        Assert.Equal(firstUnitIds, secondUnitIds);
        Assert.Equal(firstGroupIds, secondGroupIds);

        Assert.Equal(2, secondUnitIds.Count);
        Assert.Equal(13, secondGroupIds.Count);
    }

    [Fact]
    public async Task InitializeAsync_WhenRecordsAlreadyExist_PreservesThemAndAddsMissingDefaults()
    {
        // Arrange
        await using var connection = new SqliteConnection(
            "Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<StorePilotDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new StorePilotDbContext(options);

        // Apply migrations before inserting existing records.
        await dbContext.Database.MigrateAsync();

        var existingUnit = new Unit
        {
            Name = "وحدة مخصصة"
        };

        var existingGroup = new ProductGroup
        {
            Name = "مجموعة مخصصة"
        };

        dbContext.Units.Add(existingUnit);
        dbContext.ProductGroups.Add(existingGroup);

        await dbContext.SaveChangesAsync();

        var originalUnitId = existingUnit.Id;
        var originalGroupId = existingGroup.Id;

        var sut = CreateInitializer(dbContext);

        // Act
        await sut.InitializeAsync();

        // Assert
        var savedUnit = await dbContext.Units
            .SingleAsync(unit => unit.Name == "وحدة مخصصة");

        var savedGroup = await dbContext.ProductGroups
            .SingleAsync(group => group.Name == "مجموعة مخصصة");

        Assert.Equal(originalUnitId, savedUnit.Id);
        Assert.Equal(originalGroupId, savedGroup.Id);

        Assert.Equal(3, await dbContext.Units.CountAsync());
        Assert.Equal(14, await dbContext.ProductGroups.CountAsync());
    }

    private static DatabaseInitializer CreateInitializer(
        StorePilotDbContext dbContext)
    {
        return new DatabaseInitializer(
            dbContext,
            NullLogger<DatabaseInitializer>.Instance);
    }
}
