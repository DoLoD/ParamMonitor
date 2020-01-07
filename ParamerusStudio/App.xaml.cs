using DevExpress.Xpf.Core;
using System.Windows;

namespace ParamerusStudio
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            var theme = new Theme("ParamerusTheme");
            theme.AssemblyName = "DevExpress.Xpf.Themes.ParamerusTheme.v19.2";
           
            Theme.RegisterTheme(theme);
            ApplicationThemeHelper.ApplicationThemeName = "ParamerusTheme";
        }
        
            
    }
}
