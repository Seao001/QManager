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
            desktop.MainWindow = SessionState.IsSignedIn ? new MainWindow() : new LoginWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
