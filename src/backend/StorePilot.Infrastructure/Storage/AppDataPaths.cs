namespace StorePilot.Infrastructure.Storage;

public sealed class AppDataPaths
{
    public string RootPath { get; }

    public string DataDirectory { get; }

    public string LogsDirectory { get; }

    public string SettingsDirectory { get; }

    public string BrandingDirectory { get; }

    public string BackupsDirectory { get; }

    public string DatabasePath { get; }

    public string SettingsFilePath { get; }

    public AppDataPaths(
        string? rootPath = null,
        string? backupsDirectory = null)
    {
        // Use the custom root path when provided.
        // Otherwise, use the normal application data directory.
        RootPath = rootPath ?? Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "StorePilot");

        DataDirectory = Path.Combine(RootPath, "Data");

        LogsDirectory = Path.Combine(RootPath, "Logs");

        SettingsDirectory = Path.Combine(RootPath, "Settings");

        BrandingDirectory = Path.Combine(RootPath, "Assets");

        // Allow tests to use an isolated backup directory.
        // The production application keeps its existing default path.
        BackupsDirectory = backupsDirectory ?? Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments),
            "StorePilot",
            "Backups");

        DatabasePath = Path.Combine(
            DataDirectory,
            "StorePilot.db");

        SettingsFilePath = Path.Combine(
            SettingsDirectory,
            "app-settings.json");

        CreateDirectories();
    }

    private void CreateDirectories()
    {
        Directory.CreateDirectory(DataDirectory);

        Directory.CreateDirectory(LogsDirectory);

        Directory.CreateDirectory(SettingsDirectory);

        Directory.CreateDirectory(BrandingDirectory);

        Directory.CreateDirectory(BackupsDirectory);
    }
}
