namespace ZeleninCalculatorCore.Models;

public sealed class ThemeSettings
{
    public static readonly ThemeSettings Default = new(isDarkTheme: true, accentColor: "#5B7CFA");

    public bool IsDarkTheme { get; set; } = true;
    public string AccentColor { get; set; } = "#5B7CFA";

    public ThemeSettings() { }

    public ThemeSettings(bool isDarkTheme, string accentColor)
    {
        IsDarkTheme = isDarkTheme;
        AccentColor = accentColor;
    }
}

