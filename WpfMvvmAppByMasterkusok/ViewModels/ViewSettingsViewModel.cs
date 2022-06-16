using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    public class ViewSettingsViewModel : BaseViewModel
    {
        private User _user;
        private IConfigManager _configManager;
        public ICommand ApplyCommand { get; set; }
        public Dictionary<string, SolidColorBrush> SelectedColors { get; set; }
        public ViewSettingsViewModel(NavigationStore navigationStore, User user, IConfigManager manager)
        {
            _navigationStore = navigationStore;
            _user = user;
            _configManager = manager;

            ApplyCommand = new RelayCommand(obj =>
            {
                ApplyCustomTheme();
            });

            SetupColors();
        }

        private void ApplyCustomTheme()
        {
            SetAppRecourcesColors();
            SaveThemeInConfig();
        }

        private void SetAppRecourcesColors()
        {
            foreach(KeyValuePair<string, SolidColorBrush> color in SelectedColors)
            {
                if (Application.Current.Resources.Contains(color.Key))
                {
                    Application.Current.Resources[color.Key] = color.Value;
                }
            }
        }

        private void SaveThemeInConfig()
        {
            _configManager.SaveConfiguration();
        }

        private void SetupColors()
        {
            SelectedColors = new Dictionary<string, SolidColorBrush>();
            SelectedColors.Add("BGColor1", _configManager.Config.CurrentTheme.BGBrush1);
            SelectedColors.Add("BGColor2", _configManager.Config.CurrentTheme.BGBrush2);
            SelectedColors.Add("FGColor", _configManager.Config.CurrentTheme.FGBrush);
            SelectedColors.Add("OPColor", _configManager.Config.CurrentTheme.OPBrush);
        }
    }
}
