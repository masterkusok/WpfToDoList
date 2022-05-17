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

        #region Visibilities and other view variables
        private bool _controlsEnabled = true;
        private bool _loadingPopupOpened;
        private bool _loginErrorPopupOpened;
        private bool _registerErrorPopupOpened;
        private bool _isRegistrationMode = false;
        private bool _registerSuccessPopupOpened;
        private Visibility _registrationMode = Visibility.Hidden;
        private Visibility _loginMode = Visibility.Visible;
        public Visibility RegistrationMode { get => _registrationMode; set => _registrationMode = value; }
        public Visibility LoginMode { get => _loginMode; set => _loginMode = value; }
        
        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }
        public bool LoadingPopupOpened { get => _loadingPopupOpened; set => _loadingPopupOpened = value; }
        public bool LoginErrorPopupOpened { get => _loginErrorPopupOpened; set => _loginErrorPopupOpened = value;}
        public bool RegisterErrorPopupOpened { get => _registerErrorPopupOpened; set => _registerErrorPopupOpened = value; }
        public bool RegisterSuccessPopupOpened { get => _registerSuccessPopupOpened;
            set => _registerSuccessPopupOpened = value; }
        #endregion

        public string Username { get => _username; set => _username = value; }

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
            _loadingPopupOpened = true;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            NotifyOnPropertyChanged(nameof(LoadingPopupOpened));
        }
        private void UnlockAllControls()
        {
            _controlsEnabled = true;
            _loadingPopupOpened = false;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            NotifyOnPropertyChanged(nameof(LoadingPopupOpened));
        }
        private User GetUserFromDb()
        {
            return _dbService.GetUser(_username, _password);
        }

        private void ExecuteNavigation(User user)
        {
            if(user is not NotExistingUser)
            {
                MoveToPage(new MainPage(user));
                return;
            }
            DisplayErrorPopup();
        }

        private async void DisplayErrorPopup()
        {
            _loginErrorPopupOpened = true;
            NotifyOnPropertyChanged(nameof(LoginErrorPopupOpened));
            await Task.Delay(6500);
            _loginErrorPopupOpened = false;
            NotifyOnPropertyChanged(nameof(LoginErrorPopupOpened));
        }

        private async void TryToRegister(object parameter)
        {
            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (_username != null && _password != null)
            {
                bool completedSuccessfully = false;
                BlockAllControls();
                await Task.Run(async () =>
                {
                    await Task.Delay(1500);
                    if(_dbService.AddUser(_username, _password))
                    {
                        completedSuccessfully = true;
                    }
                });
                UnlockAllControls();
                if (completedSuccessfully)
                {
                    ExecuteRegisterNavigation();
                    return;
                }
                DisplayRegisterErrorPopup();
            }
        }

        private void ExecuteRegisterNavigation()
        {
            DisplayRegisterSuccessPopupAndRedirect();
        }

        private async void DisplayRegisterSuccessPopupAndRedirect()
        {
            _registerSuccessPopupOpened = true;
            NotifyOnPropertyChanged(nameof(RegisterSuccessPopupOpened));
            await Task.Delay(5000);
            _registerSuccessPopupOpened = false;
            NotifyOnPropertyChanged(nameof(RegisterSuccessPopupOpened));
            RedirectToLoginPage();
        }

        private void RedirectToLoginPage()
        {
            MoveToPage(new LoginPage(_navigationStore));
        }

        private async void DisplayRegisterErrorPopup()
        {
            // Maybe i will create PopupDisplayer class, to avoid this exactly similar functions
            _registerErrorPopupOpened = true;
            NotifyOnPropertyChanged(nameof(RegisterErrorPopupOpened));
            await Task.Delay(6500);
            _registerErrorPopupOpened = false;
            NotifyOnPropertyChanged(nameof(RegisterErrorPopupOpened));
        }
    }
}
