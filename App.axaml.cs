using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using QManager.View;
using QManager.Service;

namespace QManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ThemeManager.Initialize();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            SessionState.Restore();
            Avalonia.Controls.Window initialWindow = SessionState.IsSignedIn ? new MainWindow() : new LoginWindow();
            desktop.MainWindow = initialWindow;
            initialWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
