using System;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        User _user;
        IConfigManager _configManager;

        public SettingsViewModel(NavigationStore store, IConfigManager configManager, User user)
        {
            _user = user;
            _configManager = configManager;
            _navigationStore = store;
        }
    }
}
