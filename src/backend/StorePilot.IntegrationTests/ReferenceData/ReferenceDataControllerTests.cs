using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StorePilot.Api.Controllers;
using StorePilot.Application.Dtos.ReferenceData;
using StorePilot.Domain.Entities;
using StorePilot.Infrastructure.Data;

namespace StorePilot.IntegrationTests.ReferenceData;

public sealed class ReferenceDataControllerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly StorePilotDbContext _dbContext;

    public ReferenceDataControllerTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<StorePilotDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new StorePilotDbContext(options);

        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAllUnits_ReturnsUnitsOrderedByName()
    {
        // Arrange
        _dbContext.Units.AddRange(
            new Unit { Name = "متر" },
            new Unit { Name = "قطعة" });

        await _dbContext.SaveChangesAsync();

        var sut = new UnitsController(_dbContext);

        // Act
        var actionResult = await sut.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(
            actionResult.Result);

        var result = Assert.IsAssignableFrom<
            IReadOnlyList<ReferenceDataItemDto>>(okResult.Value);

        Assert.Equal(2, result.Count);

        Assert.Equal(
            result.Select(item => item.Name).OrderBy(name => name),
            result.Select(item => item.Name));
    }

    [Fact]
    public async Task GetUnitById_WhenUnitExists_ReturnsUnit()
    {
        // Arrange
        var unit = new Unit { Name = "متر" };

        _dbContext.Units.Add(unit);
        await _dbContext.SaveChangesAsync();

        var sut = new UnitsController(_dbContext);

        // Act
        var actionResult = await sut.GetById(
            unit.Id,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(
            actionResult.Result);

        var result = Assert.IsType<ReferenceDataItemDto>(
            okResult.Value);

        Assert.Equal(unit.Id, result.Id);
        Assert.Equal(unit.Name, result.Name);
    }

    [Fact]
    public async Task GetUnitById_WhenUnitDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var sut = new UnitsController(_dbContext);

        // Act
        var actionResult = await sut.GetById(
            999,
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task GetAllGroups_ReturnsGroupsOrderedByName()
    {
        // Arrange
        _dbContext.ProductGroups.AddRange(
            new ProductGroup { Name = "نواكل" },
            new ProductGroup { Name = "قطع بولي" });

        await _dbContext.SaveChangesAsync();

        var sut = new GroupsController(_dbContext);

        // Act
        var actionResult = await sut.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(
            actionResult.Result);

        var result = Assert.IsAssignableFrom<
            IReadOnlyList<ReferenceDataItemDto>>(okResult.Value);

        Assert.Equal(2, result.Count);

        Assert.Equal(
            result.Select(item => item.Name).OrderBy(name => name),
            result.Select(item => item.Name));
    }

    [Fact]
    public async Task GetGroupById_WhenGroupExists_ReturnsGroup()
    {
        // Arrange
        var group = new ProductGroup { Name = "نواكل" };

        _dbContext.ProductGroups.Add(group);
        await _dbContext.SaveChangesAsync();

        var sut = new GroupsController(_dbContext);

        // Act
        var actionResult = await sut.GetById(
            group.Id,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(
            actionResult.Result);

        var result = Assert.IsType<ReferenceDataItemDto>(
            okResult.Value);

        Assert.Equal(group.Id, result.Id);
        Assert.Equal(group.Name, result.Name);
    }

    [Fact]
    public async Task GetGroupById_WhenGroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var sut = new GroupsController(_dbContext);

        // Act
        var actionResult = await sut.GetById(
            999,
            CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
