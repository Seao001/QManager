using Avalonia;
using Avalonia.Styling;

namespace QManager.Service
{
    public class ThemeManager
    {
        public void SwitchToTheme(bool isDark)
        {
            var app = Application.Current;
            if (app != null)
            {
                app.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
            }
        }
    }
}