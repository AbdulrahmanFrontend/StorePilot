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
    }

    [Fact]
    public async Task GetSummary_WhenProductsExist_ReturnsActiveProductsCount()
    {
        // Arrange
        var unit = new Unit
        {
            Name = "Piece",
        };

        var group = new ProductGroup
        {
            Name = "Pipes",
        };

        _dbContext.Units.AddRange(unit);
        _dbContext.ProductGroups.AddRange(group);

        await _dbContext.SaveChangesAsync();

        _dbContext.Products.AddRange(
            new Product
            {
                Name = "Active Product",
                UnitId = unit.Id,
                GroupId = group.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Product
            {
                Name = "Inactive Product",
                UnitId = unit.Id,
                GroupId = group.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = false
            });

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await GetSummaryAsync();

        // Assert
        Assert.Equal(1, result.ActiveProductsCount);
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
