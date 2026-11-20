using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StorePilot.Api.Controllers;
using StorePilot.Application.Dtos.Dashboard;
using StorePilot.Domain.Entities;
using StorePilot.Infrastructure.Data;

namespace StorePilot.IntegrationTests.Dashboard;

public sealed class DashboardControllerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly StorePilotDbContext _dbContext;
    private readonly DashboardController _sut;

    public DashboardControllerTests()
    {
        _connection = new SqliteConnection(
            "Data Source=:memory:");

        _connection.Open();

        var options = new DbContextOptionsBuilder<StorePilotDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new StorePilotDbContext(options);

        _dbContext.Database.EnsureCreated();

        _sut = new DashboardController(_dbContext);
    }

    [Fact]
    public async Task GetSummary_WhenDatabaseIsEmpty_ReturnsZeroCounts()
    {
        // Act
        var result = await GetSummaryAsync();

        // Assert
        Assert.Equal(0, result.ActiveProductsCount);
        Assert.Equal(0, result.ActiveGroupsCount);
        Assert.Equal(0, result.ActiveUnitsCount);
    }

    [Fact]
    public async Task GetSummary_WhenActiveAndInactiveRecordsExist_ReturnsOnlyActiveCounts()
    {
        // Arrange
        var activeUnit = new Unit
        {
            Name = "Piece",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var inactiveUnit = new Unit
        {
            Name = "Box",
            CreatedAt = DateTime.UtcNow,
            IsActive = false
        };

        var activeGroup = new ProductGroup
        {
            Name = "Pipes",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var inactiveGroup = new ProductGroup
        {
            Name = "Tools",
            CreatedAt = DateTime.UtcNow,
            IsActive = false
        };

        _dbContext.Units.AddRange(activeUnit, inactiveUnit);
        _dbContext.ProductGroups.AddRange(activeGroup, inactiveGroup);

        await _dbContext.SaveChangesAsync();

        _dbContext.Products.AddRange(
            new Product
            {
                Name = "Active Product",
                UnitId = activeUnit.Id,
                GroupId = activeGroup.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Name = "Inactive Product",
                UnitId = activeUnit.Id,
                GroupId = activeGroup.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = false
            });

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await GetSummaryAsync();

        // Assert
        Assert.Equal(1, result.ActiveProductsCount);
        Assert.Equal(1, result.ActiveGroupsCount);
        Assert.Equal(1, result.ActiveUnitsCount);
    }

    private async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var actionResult = await _sut.GetSummary(
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(
            actionResult.Result);

        return Assert.IsType<DashboardSummaryDto>(
            okResult.Value);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
