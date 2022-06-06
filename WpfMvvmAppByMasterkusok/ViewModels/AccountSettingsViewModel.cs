using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class AccountSettingsViewModel : BaseViewModel
    {
        private User _user;
        private IConfigManager _configManager;
        public AccountSettingsViewModel(NavigationStore navigationStore, User user, IConfigManager manager)
        {
            _navigationStore = navigationStore;
            _user = user;
            _configManager = manager;
        }
    }
}
