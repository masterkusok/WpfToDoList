using System;
using System.Windows.Controls;
using WpfMvvmAppByMasterkusok.Views;

namespace WpfMvvmAppByMasterkusok.Stores
{
    public class NavigationStore
    {
        private Page _currentPage;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { 
                _currentPage = value;
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
            _currentPage = new LoginPage(this);
        }
    }
}
