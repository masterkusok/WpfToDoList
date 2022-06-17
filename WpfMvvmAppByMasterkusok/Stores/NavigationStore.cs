using System;
using WpfMvvmAppByMasterkusok.ViewModels;
using WpfMvvmAppByMasterkusok.Models;

namespace WpfMvvmAppByMasterkusok.Stores
{
    public class NavigationStore
    {

        private BaseViewModel _currentVM;
        public BaseViewModel CurrentVM
        {
            get { return _currentVM; }
            set { 
                _currentVM = value;
                OnCurrentPageChanged();
            }
        }
        private void OnCurrentPageChanged()
        {
            CurrentPageChanged?.Invoke();
        }
        public event Action CurrentPageChanged;

        public NavigationStore()
        {
            JsonConfigManager jsonConfigManager = new JsonConfigManager();
            jsonConfigManager.LoadConfiguration();
            if(jsonConfigManager.Config.LoginedUser != null)
            {
                _currentVM = new MainPageViewModel(this, jsonConfigManager.Config.LoginedUser, jsonConfigManager);
                return;
            }
            _currentVM = new LoginViewModel(this, jsonConfigManager);
        }
    }
}
