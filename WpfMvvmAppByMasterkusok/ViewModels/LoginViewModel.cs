using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Views;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private NavigationStore _navigationStore;
        public ICommand LoginCommand
        {
            get;
            set;
        } 
        public LoginViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            LoginCommand = new RelayCommand(obj =>
            {
                _navigationStore.CurrentPage = new MainPage();
            });
        }

    }
}
