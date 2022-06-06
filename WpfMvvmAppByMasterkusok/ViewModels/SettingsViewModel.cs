using System;
using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        User _user;
        IConfigManager _configManager;
        public ICommand GoBackCommand { get; set; }
        public AccountSettingsViewModel CurrentAccountSettingsView { get; set; }

        public SettingsViewModel(NavigationStore store, IConfigManager configManager, User user)
        {
            _user = user;
            _configManager = configManager;
            _navigationStore = store;

            CurrentAccountSettingsView = new AccountSettingsViewModel(_navigationStore, _user, _configManager);

            GoBackCommand = new RelayCommand(obj =>
            {
                GoBackToMainPage();
            });
        }

        private void GoBackToMainPage()
        {
            // TODO: Rework this function later, to setup already existing MainPageViewModel
            ChangeCurrentVM(new MainPageViewModel(_navigationStore, _user, _configManager));
        }

    }
}
