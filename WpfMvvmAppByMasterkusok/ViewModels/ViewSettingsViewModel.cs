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

        public PopupRepresenter HaveToReloadMessagePopup { get; set; }
        public ICommand ApplyCommand { get; set; }
        public Dictionary<string, SolidColorBrush> SelectedColors { get; set; }
        public string SelectedThemeTag { get; set; }
        public ViewSettingsViewModel(NavigationStore navigationStore, User user, IConfigManager manager)
        {
            _navigationStore = navigationStore;
            _user = user;
            _configManager = manager;

            HaveToReloadMessagePopup = new PopupRepresenter(nameof(HaveToReloadMessagePopup), this);

            ApplyCommand = new RelayCommand(obj =>
            {
                ApplyChanges();
            });

            SelectedThemeTag = string.Empty;
            SetupColors();
        }

        private void ApplyChanges()
        {
            switch (SelectedThemeTag)
            {
                case "Dark":
                    {
                        ApplyDarkTheme();
                        break;
                    }
                case "Light":
                    {
                        ApplyLightTheme();
                        break;
                    }
                case "Custom":
                    {
                        ApplyCustomTheme();
                        break;
                    }
            }
            HaveToReloadMessagePopup.ShowWithTimer(5000);
        }

        private void ApplyDarkTheme()
        {
            _configManager.Config.CurrentTheme = Theme.DarkTheme;
            _configManager.SaveConfiguration();
        }

        private void ApplyLightTheme()
        {
            _configManager.Config.CurrentTheme = Theme.LightTheme;
            _configManager.SaveConfiguration();
        }

        private void ApplyCustomTheme()
        {
            _configManager.Config.CurrentTheme.BGBrush1 = SelectedColors["BGColor1"];
            _configManager.Config.CurrentTheme.BGBrush2 = SelectedColors["BGColor2"];
            _configManager.Config.CurrentTheme.FGBrush = SelectedColors["FGColor"];
            _configManager.Config.CurrentTheme.OPBrush = SelectedColors["OPColor"];
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
