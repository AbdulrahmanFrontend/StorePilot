namespace StorePilot.Infrastructure.Storage;

public sealed class AppDataPaths
{
    public string RootPath { get; }

    public string DataDirectory { get; }

    public string LogsDirectory { get; }

    public string SettingsDirectory { get; }

    public string AssetsDirectory { get; }

    public string BackupsDirectory { get; }

    public string DatabasePath { get; }

    public string SettingsFilePath { get; }

    public AppDataPaths()
    {
        RootPath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "StorePilot");

        DataDirectory = Path.Combine(RootPath, "Data");
        LogsDirectory = Path.Combine(RootPath, "Logs");
        SettingsDirectory = Path.Combine(RootPath, "Settings");
        AssetsDirectory = Path.Combine(RootPath, "Assets");
        BackupsDirectory = Path.Combine(
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
        Directory.CreateDirectory(AssetsDirectory);
        Directory.CreateDirectory(BackupsDirectory);
    }
}
