using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using WpfMvvmAppByMasterkusok.ViewModels;
using WpfMvvmAppByMasterkusok.Stores;
namespace WpfMvvmAppByMasterkusok.Views
{
    [ValueConversion(typeof(BaseViewModel), typeof(Page))]
    public class ViewModeToViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is LoginViewModel)
            {
                return new LoginPage((LoginViewModel)value);
            }
            else if(value is MainPageViewModel)
            {
                return new MainPage((MainPageViewModel)value);
            }
            else if (value is SettingsViewModel)
            {
                return new SettingsPage((SettingsViewModel)value);
            }
            else if (value is AccountSettingsViewModel)
            {
                return new AccountSettingsView((AccountSettingsViewModel)value);
            }
            else if(value is ViewSettingsViewModel)
            {
                return new ViewSettingsView((ViewSettingsViewModel)value);
            }
            else
            {
                throw new Exception("Client is trying to switch to unknown viewmodel.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
