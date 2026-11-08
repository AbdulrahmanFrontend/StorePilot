using StorePilot.Application.Dtos.Settings;

namespace StorePilot.Application.Intefaces;

public interface IStoreSettingsService
{
    Task<StoreSettingsDto> GetAsync(
        CancellationToken cancellationToken = default);

    Task<bool> IsSetupCompletedAsync(
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        string shopName,
        string ownerName,
        CancellationToken cancellationToken = default);

    Task SaveLogoAsync(
        byte[] content,
        string extension,
        CancellationToken cancellationToken = default);

    Task<StoreLogoContent?> GetLogoAsync(
        CancellationToken cancellationToken = default);

    Task DeleteLogoAsync(
        CancellationToken cancellationToken = default);
}
