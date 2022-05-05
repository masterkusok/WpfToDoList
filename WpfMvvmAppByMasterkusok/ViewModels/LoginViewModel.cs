using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Views;
using WpfMvvmAppByMasterkusok.Models;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private NavigationStore _navigationStore;
        private string _password;
        private string _username;
        private bool _controlsEnabled = true;
        private bool _popupOpened;
        private bool _errorPopupOpened;
        public string Username { get =>  _username; set => _username = value; }
        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }
        public bool PopupOpened { get => _popupOpened; set => _popupOpened = value; }
        public bool ErrorPopupOpened { get => _errorPopupOpened; set => _errorPopupOpened = value;}
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

        private async void LoginBtnClicked(object parameter)
        {
            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (_username != null && _password != null)
            {
                BlockAllControls();
                User user = null;
                await Task.Run(async () =>
                {
                    await Task.Delay(1500);
                    user = GetUserFromDb();
                });
                ExecuteNavigation(user);
                UnlockAllControls();
                return;
            }
            DisplayErrorPopup();
        }
        private void GetPasswordFromPasswordBox(PasswordBox box)
        {
            _password = box.Password;
        }
        private void BlockAllControls()
        {
            _controlsEnabled = false;
            _popupOpened = true;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            NotifyOnPropertyChanged(nameof(PopupOpened));
        }
        private void UnlockAllControls()
        {
            _controlsEnabled = true;
            _popupOpened = false;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            NotifyOnPropertyChanged(nameof(PopupOpened));
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
            DisplayErrorPopup();
        }

        private async void DisplayErrorPopup()
        {
            _errorPopupOpened = true;
            NotifyOnPropertyChanged(nameof(ErrorPopupOpened));
            await Task.Delay(6500);
            _errorPopupOpened = false;
            NotifyOnPropertyChanged(nameof(ErrorPopupOpened));
        }
    }
}
