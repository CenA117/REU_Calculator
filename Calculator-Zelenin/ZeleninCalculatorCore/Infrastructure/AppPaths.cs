namespace ZeleninCalculatorCore.Infrastructure;

internal static class AppPaths
{
    private const string AppFolderName = "ZeleninCalculator";

    public static string GetFilePath(string fileName)
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(baseDir, AppFolderName, fileName);
    }
}