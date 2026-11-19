using Microsoft.Extensions.Logging.Abstractions;
using StorePilot.Infrastructure.Settings;
using StorePilot.Infrastructure.Storage;

namespace StorePilot.UnitTests.Settings;

public sealed class JsonStoreSettingsServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly AppDataPaths _paths;
    private readonly JsonStoreSettingsService _sut;

    public JsonStoreSettingsServiceTests()
    {
        // Create a unique directory for each test.
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "StorePilot.UnitTests",
            Guid.NewGuid().ToString("N"));

        // Keep all test data isolated from real application data.
        var rootPath = Path.Combine(_testDirectory, "AppData");

        var backupsPath = Path.Combine(_testDirectory, "Backups");

        // Create application paths inside the temporary directory.
        _paths = new AppDataPaths(
            rootPath,
            backupsPath);

        // Create the real service with a logger that does nothing.
        _sut = new JsonStoreSettingsService(
            _paths,
            NullLogger<JsonStoreSettingsService>.Instance);
    }

    [Fact]
    public async Task GetAsync_WhenSettingsFileDoesNotExist_ReturnsDefaultSettings()
    {
        // Act
        var result = await _sut.GetAsync();

        // Assert
        Assert.Equal(string.Empty, result.ShopName);
        Assert.Equal(string.Empty, result.OwnerName);
        Assert.Null(result.LogoUrl);
        Assert.False(result.IsSetupCompleted);
    }

    [Fact]
    public async Task IsSetupCompletedAsync_WhenSettingsFileDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.IsSetupCompletedAsync();

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("", "Owner")]
    [InlineData("Shop", "")]
    [InlineData(" ", "Owner")]
    [InlineData("Shop", " ")]
    public async Task IsSetupCompletedAsync_WhenShopNameOrOwnerNameIsMissing_ReturnsFalse(
        string shopName,
        string ownerName)
    {
        // Arrange
        await _sut.SaveAsync(shopName, ownerName);

        // Act
        var result = await _sut.IsSetupCompletedAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SaveAsync_WhenValidNamesAreProvided_SavesTrimmedNames()
    {
        // Arrange
        const string shopName = "  StorePilot Shop  ";
        const string ownerName = "  Store Owner  ";

        // Act
        await _sut.SaveAsync(shopName, ownerName);

        var result = await _sut.GetAsync();

        // Assert
        Assert.Equal("StorePilot Shop", result.ShopName);
        Assert.Equal("Store Owner", result.OwnerName);
        Assert.True(result.IsSetupCompleted);
        Assert.Null(result.LogoUrl);
    }

    [Fact]
    public async Task SaveLogoAsync_WhenValidContentIsProvided_SavesAndReturnsLogo()
    {
        // Arrange
        byte[] logoContent = [1, 2, 3, 4, 5];

        // Act
        await _sut.SaveLogoAsync(logoContent, ".png");

        var result = await _sut.GetLogoAsync();

        var settings = await _sut.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(logoContent, result.Content);
        Assert.Equal("image/png", result.ContentType);

        Assert.NotNull(settings.LogoUrl);
        Assert.Equal("/api/system/settings/logo", settings.LogoUrl);

        var logoFiles = Directory.GetFiles(
            _paths.BrandingDirectory,
            "logo-*.png");

        Assert.Single(logoFiles);
    }

    [Fact]
    public async Task SaveLogoAsync_WhenLogoAlreadyExists_ReplacesOldLogo()
    {
        // Arrange
        byte[] firstLogo = [1, 2, 3];
        byte[] secondLogo = [4, 5, 6];

        await _sut.SaveLogoAsync(firstLogo, ".png");

        var oldLogoFiles = Directory.GetFiles(
            _paths.BrandingDirectory,
            "logo-*.png");

        Assert.Single(oldLogoFiles);

        // Act
        await _sut.SaveLogoAsync(secondLogo, ".png");

        var newLogoFiles = Directory.GetFiles(
            _paths.BrandingDirectory,
            "logo-*.png");

        var result = await _sut.GetLogoAsync();

        // Assert
        Assert.Single(newLogoFiles);
        Assert.NotEqual(oldLogoFiles[0], newLogoFiles[0]);

        Assert.NotNull(result);
        Assert.Equal(secondLogo, result.Content);
    }

    [Fact]
    public async Task DeleteLogoAsync_WhenLogoExists_DeletesLogoAndClearsReference()
    {
        // Arrange
        byte[] logoContent = [1, 2, 3];

        await _sut.SaveLogoAsync(logoContent, ".png");

        // Act
        await _sut.DeleteLogoAsync();

        var result = await _sut.GetLogoAsync();

        var settings = await _sut.GetAsync();

        var logoFiles = Directory.GetFiles(
            _paths.BrandingDirectory,
            "logo-*");

        // Assert
        Assert.Null(result);
        Assert.Null(settings.LogoUrl);
        Assert.Empty(logoFiles);
    }

    [Fact]
    public async Task GetAsync_WhenReferencedLogoFileDoesNotExist_ReturnsNullLogoUrl()
    {
        // Arrange
        await _sut.SaveLogoAsync([1, 2, 3], ".png");

        var logoFiles = Directory.GetFiles(
            _paths.BrandingDirectory,
            "logo-*.png");

        Assert.Single(logoFiles);

        File.Delete(logoFiles[0]);

        // Act
        var result = await _sut.GetAsync();

        // Assert
        Assert.Null(result.LogoUrl);
    }
    
[Fact]
public async Task DeleteLogoAsync_WhenReferencedLogoFileDoesNotExist_ClearsReference()
{
    // Arrange
    await _sut.SaveLogoAsync([1, 2, 3], ".png");

    var logoFiles = Directory.GetFiles(
        _paths.BrandingDirectory,
        "logo-*.png");

    Assert.Single(logoFiles);

    File.Delete(logoFiles[0]);

    // Act
    await _sut.DeleteLogoAsync();

    var settings = await _sut.GetAsync();

    // Assert
    Assert.Null(settings.LogoUrl);
    Assert.Null(await _sut.GetLogoAsync());
}

    public void Dispose()
    {
        // Remove all temporary files created by this test.
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, recursive: true);
    }
}
