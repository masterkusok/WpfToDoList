using System;
using System.Windows.Controls;
using WpfMvvmAppByMasterkusok.Views;
using WpfMvvmAppByMasterkusok.ViewModels;

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
            _currentVM = new LoginViewModel(this);
        }
    }
}
