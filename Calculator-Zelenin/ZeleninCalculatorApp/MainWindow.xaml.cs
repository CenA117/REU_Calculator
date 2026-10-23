using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ZeleninCalculatorApp.ViewModels;

namespace ZeleninCalculatorApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int DwmUseImmersiveDarkMode = 20;
    private const int DwmUseImmersiveDarkModeLegacy = 19;

    public MainWindow()
    {
        InitializeComponent();

        var viewModel = new MainViewModel();
        DataContext = viewModel;

        Closing += (_, _) => viewModel.Persist();
        Loaded += (_, _) => ApplyTitleBarTheme();

        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.PageBackground))
                ApplyTitleBarTheme();
        };
    }

    private void ApplyTitleBarTheme()
    {
        try
        {
            var handle = new WindowInteropHelper(this).Handle;
            if (handle == IntPtr.Zero || DataContext is not MainViewModel vm)
                return;

            int useDarkMode = IsDarkColor(vm.PageBackground) ? 1 : 0;

            if (DwmSetWindowAttribute(handle, DwmUseImmersiveDarkMode, ref useDarkMode, sizeof(int)) != 0)
                DwmSetWindowAttribute(handle, DwmUseImmersiveDarkModeLegacy, ref useDarkMode, sizeof(int));
        }
        catch
        {
            // Оформление заголовка недоступно на старых версиях Windows — не критично.
        }
    }

    private static bool IsDarkColor(string hex)
    {
        var value = hex.TrimStart('#');
        if (value.Length != 6)
            return true;

        int r = Convert.ToInt32(value[..2], 16);
        int g = Convert.ToInt32(value[2..4], 16);
        int b = Convert.ToInt32(value[4..6], 16);

        return (0.299 * r + 0.587 * g + 0.114 * b) < 128;
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int attributeValue, int attributeSize);

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (DataContext is not MainViewModel vm || string.IsNullOrEmpty(e.Text))
            return;

        char ch = e.Text[0];
        if (char.IsDigit(ch) || ch is '.' or ',' or '+' or '-' or '*' or '/' or '^' or '(' or ')')
        {
            vm.AppendToken(e.Text);
            e.Handled = true;
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        if (e.Key is Key.Enter or Key.Return)
        {
            if (vm.EvaluateCommand.CanExecute(null))
                vm.EvaluateCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Back)
        {
            vm.Backspace();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            if (vm.ClearAllCommand.CanExecute(null))
                vm.ClearAllCommand.Execute(null);
            e.Handled = true;
        }
    }
}
