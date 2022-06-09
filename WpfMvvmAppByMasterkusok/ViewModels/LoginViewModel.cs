using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Models;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private string _password;
        private string _username;

        private bool _controlsEnabled = true;
        private bool _isRegistrationMode = false;
        private bool _rememberUser = false;

        #region Popups
        private PopupRepresenter _loginErrorPopup;
        private PopupRepresenter _loadingPopup;
        private PopupRepresenter _registerPopup;
        private PopupRepresenter _registerSuccessPopup;
        private PopupRepresenter _registerErrorPopup;
        private PopupRepresenter _dbConnectionErrorPopup;

        public PopupRepresenter LoadingPopup { get => _loadingPopup; set => _loadingPopup = value; }
        public PopupRepresenter RegisterPopup { get => _registerPopup; set => _registerPopup = value; }
        public PopupRepresenter RegisterSuccessPopup { get => _registerSuccessPopup; set => _registerSuccessPopup = value; }
        public PopupRepresenter RegisterErrorPopup { get => _registerErrorPopup; set => _registerErrorPopup = value; }
        public PopupRepresenter LoginErrorPopup { get => _loginErrorPopup; set => _loginErrorPopup = value;}
        public PopupRepresenter DBConnectionErrorPopup { get => _dbConnectionErrorPopup;
            set => _dbConnectionErrorPopup = value; }
        #endregion

        public bool RegistrationMode { get => _isRegistrationMode; set => _isRegistrationMode = value; }
        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }
        public bool RememberUser { get => _rememberUser; set => _rememberUser = value; }

        public string Username { get => _username; set => _username = value; }

        private IDbService _dbService;
        private IConfigManager _configManager;

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand SwitchRegisterMode { get; set; }

        public LoginViewModel(NavigationStore navigationStore, IConfigManager configManager)
        {
            _loadingPopup = new PopupRepresenter(nameof(LoadingPopup), this);
            _registerSuccessPopup = new PopupRepresenter(nameof(RegisterSuccessPopup), this);
            _registerErrorPopup = new PopupRepresenter(nameof(RegisterErrorPopup), this);
            _registerPopup = new PopupRepresenter(nameof(RegisterPopup), this);
            _loginErrorPopup = new PopupRepresenter(nameof(LoginErrorPopup), this);
            _dbConnectionErrorPopup = new PopupRepresenter(nameof(DBConnectionErrorPopup), this);

            _navigationStore = navigationStore;
            _dbService = new SqlService();
            _configManager = configManager;

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
                _isRegistrationMode = true;
            }
            else
            {
                _isRegistrationMode = false;
            }
            NotifyOnPropertyChanged(nameof(RegistrationMode));
        }

        private async void LoginBtnClicked(object parameter)
        {
            await Task.Run(async() => 
            {
                BlockAllControls();
                if (!_dbService.CanBeConnected()) {
                    DisplayDBConnectionErrorPopup();
                    return;
                }

                GetPasswordFromPasswordBox((PasswordBox)parameter);
                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    
                    User user = null;
                    await Task.Delay(1500);
                    if (_dbService.CheckUserExists(_username, _password))
                    {
                        user = GetUserFromDb();
                    }
                    
                    if (user != null)
                    {
                        SaveLoginedUserToConfigIfNeccessary(user);
                        ChangeCurrentVM(new MainPageViewModel(_navigationStore, user, _configManager));
                        return;
                    }
                    UnlockAllControls();
                    DisplayLoginErrorPopup();
                    return;
                }
                DisplayLoginErrorPopup();
                UnlockAllControls();
            });

        }

        private async void DisplayDBConnectionErrorPopup()
        {
            _dbConnectionErrorPopup.Open();
            await Task.Delay(6500);
            _dbConnectionErrorPopup.Close();
        }

        private void SaveLoginedUserToConfigIfNeccessary(User user)
        {
            if (_rememberUser)
            {
                _configManager.Config.LoginedUser = user;
                _configManager.SaveConfiguration();
            }
        }


        private void GetPasswordFromPasswordBox(PasswordBox box)
        {
            _password = box.Password;
        }

        private void BlockAllControls()
        {
            _controlsEnabled = false;
            _loadingPopup.Open();
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }

        private void UnlockAllControls()
        {
            _controlsEnabled = true;
            _loadingPopup.Close();
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }

        private User GetUserFromDb()
        {
            return _dbService.GetUser(_username, _password);
        }


        private async void DisplayLoginErrorPopup()
        {
            _loginErrorPopup.Open();
            await Task.Delay(6500);
            _loginErrorPopup.Close();
        }

        private async void TryToRegister(object parameter)
        {
            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                bool completedSuccessfully = false;
                BlockAllControls();
                await Task.Run(async () =>
                {
                    await Task.Delay(1500);
                    if (_dbService.AddUser(_username, _password))
                    {
                        completedSuccessfully = true;
                    }
                });
                DisplayOneOfRegisterPopups(completedSuccessfully);
                UnlockAllControls();   
            }
            DisplayOneOfRegisterPopups(false);
        }
        
        private void DisplayOneOfRegisterPopups(bool completedSuccessfully)
        {
            if (completedSuccessfully)
            {
                DisplayRegisterSuccessPopupAndRedirect();
                return;
            }
            DisplayRegisterErrorPopup();
        }

        private async void DisplayRegisterSuccessPopupAndRedirect()
        {
            _registerSuccessPopup.Open();
            await Task.Delay(5000);
            _registerSuccessPopup.Close();
            ChangeCurrentVM(new LoginViewModel(_navigationStore, _configManager));
        }

        private async void DisplayRegisterErrorPopup()
        {
            _registerErrorPopup.Open();
            await Task.Delay(6500);
            _registerErrorPopup.Close();
        }
    }
}
