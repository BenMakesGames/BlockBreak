namespace BlockBreak.Services;

public static class FileSystemHelpers
{
    private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static readonly string GameDataPath = $"{AppDataPath}{Path.DirectorySeparatorChar}BlockBreak";

    public static void EnsureDirectoryExists()
    {
        Directory.CreateDirectory(GameDataPath);
    }
}