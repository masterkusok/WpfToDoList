using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Views;
using WpfMvvmAppByMasterkusok.Models;
using System.Windows.Controls;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private NavigationStore _navigationStore;
        private string _password;
        private string _username;
        public string Username { get =>  _username; set => _username = value; }
        private IDbService _dbService;
        public ICommand LoginCommand
        {
            get;
            set;
        } 
        public LoginViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _dbService = new SqlService();
            LoginCommand = new RelayCommand(obj =>
            {
                LoginBtnClicked(obj);
            });
        }

        private void LoginBtnClicked(object parameter)
        {
            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (_username != null && _password != null)
            {
                User user = GetUserFromDb();
                ExecuteNavigation(user);
            }
        }
        private void GetPasswordFromPasswordBox(PasswordBox box)
        {
            _password = box.Password;
        }

        private User GetUserFromDb()
        {
            return _dbService.GetUser(_username, _password);
        }

        private void ExecuteNavigation(User user)
        {
            if(user != null)
            {
                _navigationStore.CurrentPage = new MainPage();
                return;
            }
        }
    }
}
