using WpfMvvmAppByMasterkusok.ViewModels;
using System.Windows;
using System.Windows.Media;

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
            Application.Current.Resources.Add("HTBGColor2", GetHalfTransparentBrush(_appVM.Config.CurrentTheme.BGBrush2));
            Application.Current.Resources.Add("FGColor", _appVM.Config.CurrentTheme.FGBrush);
            Application.Current.Resources.Add("OPColor", _appVM.Config.CurrentTheme.OPBrush);
        }

        private SolidColorBrush GetHalfTransparentBrush(SolidColorBrush brush)
        {
            SolidColorBrush htColor = new SolidColorBrush();
            htColor.Color = _appVM.Config.CurrentTheme.BGBrush2.Color;
            htColor.Opacity = 0.8;
            return htColor;
        }

    }
}
