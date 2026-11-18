using Microsoft.Extensions.Logging;
using StorePilot.Application.Dtos.Settings;
using StorePilot.Application.Interfaces;
using StorePilot.Infrastructure.Storage;
using System.Text.Json;

namespace StorePilot.Infrastructure.Settings;

public sealed class JsonStoreSettingsService(
    AppDataPaths appDataPaths,
    ILogger<JsonStoreSettingsService> logger) : IStoreSettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly string _settingsFilePath =
        appDataPaths.SettingsFilePath;

    private readonly string _brandingDirectory =
        appDataPaths.BrandingDirectory;

    public async Task<StoreSettingsDto> GetAsync(
        CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var settings = await ReadAsync(cancellationToken);

            return ToDto(settings);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> IsSetupCompletedAsync(
        CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var settings = await ReadAsync(cancellationToken);

            return IsSetupCompleted(settings);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SaveAsync(
        string shopName,
        string ownerName,
        CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var settings = await ReadAsync(cancellationToken);

            settings.ShopName = shopName.Trim();
            settings.OwnerName = ownerName.Trim();
            settings.UpdatedAtUtc = DateTime.UtcNow;

            await WriteAsync(settings, cancellationToken);

            logger.LogInformation("Store settings updated.");
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SaveLogoAsync(
        byte[] content,
        string extension,
        CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        string? newLogoPath = null;

        bool settingsSaved = false;

        try
        {
            var settings = await ReadAsync(cancellationToken);

            var oldLogoFileName = settings.LogoFileName;

            var newLogoFileName =
                $"logo-{Guid.NewGuid():N}{extension}";

            newLogoPath = GetLogoPath(newLogoFileName);

            await File.WriteAllBytesAsync(
                newLogoPath,
                content,
                cancellationToken);

            settings.LogoFileName = newLogoFileName;
            settings.UpdatedAtUtc = DateTime.UtcNow;

            await WriteAsync(settings, cancellationToken);

            settingsSaved = true;

            if (!string.IsNullOrWhiteSpace(oldLogoFileName))
            {
                try
                {
                    DeleteLogoFile(oldLogoFileName);
                }
                catch (IOException ex)
                {
                    logger.LogWarning(
                        ex,
                        "Failed to delete the previous logo file: {LogoFileName}",
                        oldLogoFileName);
                }
                catch (UnauthorizedAccessException ex)
                {
                    logger.LogWarning(
                        ex,
                        "Access denied while deleting the previous logo file: {LogoFileName}",
                        oldLogoFileName);
                }
            }

            logger.LogInformation(
                "Store logo updated: NewLogoFileName = '{NewLogoFileName}'",
                newLogoFileName);
        }
        catch
        {
            if (!settingsSaved && newLogoPath is not null)
            {
                try
                {
                    File.Delete(newLogoPath);
                }
                catch (IOException ex)
                {
                    logger.LogWarning(
                        ex,
                        "Failed to clean up the new logo file after an unsuccessful settings save: {LogoFileName}",
                        Path.GetFileName(newLogoPath));
                }
                catch (UnauthorizedAccessException ex)
                {
                    logger.LogWarning(
                        ex,
                        "Access denied while cleaning up the new logo file: {LogoFileName}",
                        Path.GetFileName(newLogoPath));
                }
            }

            throw;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<StoreLogoContent?> GetLogoAsync(
        CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var settings = await ReadAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(settings.LogoFileName))
                return null;

            var path = GetLogoPath(settings.LogoFileName);

            if (!File.Exists(path))
                return null;

            var content = await File.ReadAllBytesAsync(
                path,
                cancellationToken);

            var contentType = GetContentType(
                Path.GetExtension(path));

            return new StoreLogoContent(
                content,
                contentType);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteLogoAsync(
        CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var settings = await ReadAsync(cancellationToken);

            var oldLogoFileName = settings.LogoFileName;

            if (string.IsNullOrWhiteSpace(oldLogoFileName))
                return;

            settings.LogoFileName = null;
            settings.UpdatedAtUtc = DateTime.UtcNow;

            await WriteAsync(settings, cancellationToken);

            try
            {
                DeleteLogoFile(oldLogoFileName);
            }
            catch (IOException ex)
            {
                logger.LogWarning(
                    ex,
                    "Failed to delete the logo file after removing its reference from store settings: {LogoFileName}",
                    oldLogoFileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(
                    ex,
                    "Access denied while deleting the logo file: {LogoFileName}",
                    oldLogoFileName);
            }

            logger.LogInformation(
                "Store logo removed successfully.");
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<StoreSettingsFile> ReadAsync(
        CancellationToken cancellationToken)
    {
        if (!File.Exists(_settingsFilePath))
            return new StoreSettingsFile();

        await using var stream = new FileStream(
            _settingsFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        return await JsonSerializer.DeserializeAsync<StoreSettingsFile>(
                   stream,
                   JsonOptions,
                   cancellationToken)
               ?? new StoreSettingsFile();
    }

    private void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(
                ex,
                "Failed to delete temporary file: {FileName}",
                Path.GetFileName(path));
        }
    }

    private async Task WriteAsync(
        StoreSettingsFile settings,
        CancellationToken cancellationToken)
    {
        var temporaryPath =
            $"{_settingsFilePath}.{Guid.NewGuid():N}.tmp";

        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    settings,
                    JsonOptions,
                    cancellationToken);

                await stream.FlushAsync(cancellationToken);
            }

            // استبدال الملف بعد اكتمال الكتابة.
            File.Move(
                temporaryPath,
                _settingsFilePath,
                overwrite: true);
        }
        finally
        {
            TryDeleteFile(temporaryPath);
        }
    }

    private StoreSettingsDto ToDto(StoreSettingsFile settings)
    {
        var hasLogo =
            !string.IsNullOrWhiteSpace(settings.LogoFileName) &&
            File.Exists(GetLogoPath(settings.LogoFileName));

        return new StoreSettingsDto(
            settings.ShopName,
            settings.OwnerName,
            hasLogo ? "/api/system/settings/logo" : null,
            IsSetupCompleted(settings));
    }

    private static bool IsSetupCompleted(
        StoreSettingsFile settings)
    {
        return !string.IsNullOrWhiteSpace(settings.ShopName)
            && !string.IsNullOrWhiteSpace(settings.OwnerName);
    }

    private string GetLogoPath(string fileName)
    {
        // لا نسمح باستخدام مسار قادم من المستخدم.
        var safeFileName = Path.GetFileName(fileName);

        return Path.Combine(
            _brandingDirectory,
            safeFileName);
    }

    private void DeleteLogoFile(string fileName)
    {
        var path = GetLogoPath(fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
