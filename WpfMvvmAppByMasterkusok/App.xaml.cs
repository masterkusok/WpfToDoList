using WpfMvvmAppByMasterkusok.ViewModels;
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
            Application.Current.Resources.Add("BGColor1",  _appVM.Config.CurrentTheme.BGBrush1);
            Application.Current.Resources.Add("BGColor2", _appVM.Config.CurrentTheme.BGBrush2);
            Application.Current.Resources.Add("FGColor", _appVM.Config.CurrentTheme.FGBrush);
            Application.Current.Resources.Add("OPColor", _appVM.Config.CurrentTheme.OPBrush);
        }

    }
}
