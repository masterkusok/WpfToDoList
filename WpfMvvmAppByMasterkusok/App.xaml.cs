
using WpfMvvmAppByMasterkusok.ViewModels;
using System;
using System.Windows;

namespace WpfMvvmAppByMasterkusok
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        AppViewModel _appVM = new AppViewModel();
        App()
        {
           SetThemeColors();
        }

        private void SetThemeColors()
        {
            Application.Current.Resources["BGColor1"] = _appVM.CurrentTheme.BGBrush1;
            Application.Current.Resources["BGColor2"] = _appVM.CurrentTheme.BGBrush2;
            Application.Current.Resources["FGColor"] = _appVM.CurrentTheme.FGBrush;
            Application.Current.Resources["OPColor"] = _appVM.CurrentTheme.OPBrush;
        }
    }
}
