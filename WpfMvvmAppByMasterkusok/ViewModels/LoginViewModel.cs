using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Views;
using WpfMvvmAppByMasterkusok.Models;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;
using System;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private string _password;
        private string _username;
        private bool _controlsEnabled = true;
        private bool _popupOpened;
        private bool _errorPopupOpened;
        private bool _isRegistrationMode = false;
        private Visibility _registrationMode = Visibility.Hidden;
        private Visibility _loginMode = Visibility.Visible;
        public Visibility RegistrationMode { get => _registrationMode; set => _registrationMode = value; }
        public Visibility LoginMode { get => _loginMode; set => _loginMode = value; }
        public string Username { get =>  _username; set => _username = value; }
        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }
        public bool PopupOpened { get => _popupOpened; set => _popupOpened = value; }
        public bool ErrorPopupOpened { get => _errorPopupOpened; set => _errorPopupOpened = value;}

        private IDbService _dbService;
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand SwitchRegisterMode { get; set; }
        public LoginViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _dbService = new SqlService();
            LoginCommand = new RelayCommand(obj =>
            {
                LoginBtnClicked(obj);
            });
            SwitchRegisterMode = new RelayCommand(obj =>
            {
                SwitchRegisterModeClicked();
            });
            RegisterCommand = new RelayCommand(obj =>
            {
                TryToRegister(obj);
            });
        }

        private void SwitchRegisterModeClicked()
        {
            if (!_isRegistrationMode)
            {
                _registrationMode = Visibility.Visible;
                _loginMode = Visibility.Hidden;
                _isRegistrationMode = true;
            }
            else
            {
                _registrationMode = Visibility.Hidden;
                _loginMode = Visibility.Visible;
                _isRegistrationMode = false;
            }
            NotifyOnPropertyChanged(nameof(RegistrationMode));
            NotifyOnPropertyChanged(nameof(LoginMode));
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
                MoveToPage(new MainPage(user));
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

        private async void TryToRegister(object parameter)
        {
            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (_username != null && _password != null)
            {
                BlockAllControls();
                await Task.Run(async () =>
                {
                    await Task.Delay(1500);
                    if(_dbService.AddUser(_username, _password))
                    {
                        return;
                    };
                });
                UnlockAllControls();
            }
            //DisplayErrorPopup();
        }
    }
}
