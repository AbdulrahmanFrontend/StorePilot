namespace StorePilot.Infrastructure.Settings;

internal sealed class StoreSettingsFile
{
    public string ShopName { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public string? LogoFileName { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
