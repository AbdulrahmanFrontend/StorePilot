namespace StorePilot.Application.Dtos.Settings;

public sealed record StoreSettingsDto(
    string ShopName,
    string OwnerName,
    string? LogoUrl,
    bool IsSetupCompleted);
