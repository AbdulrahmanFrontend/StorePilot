namespace StorePilot.Application.Dtos.Settings;

public sealed record StoreLogoContent(
    byte[] Content,
    string ContentType);
