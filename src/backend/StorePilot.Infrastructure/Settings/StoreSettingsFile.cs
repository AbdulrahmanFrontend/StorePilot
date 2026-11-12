namespace StorePilot.Infrastructure.Settings;

internal sealed class StoreSettingsFile
{
    public string ShopName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string? LogoFileName { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
