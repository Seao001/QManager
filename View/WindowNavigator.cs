using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace QManager.View
{
    public static class WindowNavigator
    {
        public static void Open<TWindow>(Window current) where TWindow : Window, new()
        {
            Open(current, new TWindow());
        }

        public static void Open(Window current, Window next)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = next;
            }

            next.Show();
            current.Close();
        }
    }
}
