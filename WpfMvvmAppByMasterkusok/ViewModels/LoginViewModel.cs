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
        private bool _canConnectToDB = false;

        private User _loginedUser;
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

            _canConnectToDB = _dbService.CanBeConnected();

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
            BlockAllControls();

            // This delay allows user to enjoy beautiful loader animation UwU
            await Task.Delay(1500);

            await Task.Run(() => {
                _canConnectToDB = _dbService.CanBeConnected();
            });

            if (!_canConnectToDB)
            {
                _dbConnectionErrorPopup.ShowWithTimer(5000);
                UnlockAllControls();
                return;
            }

            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (CheckIfUsernameAndPasswordAreValid())
            {
                _loginErrorPopup.ShowWithTimer(5000);
                UnlockAllControls();
                return;
            }

            await Task.Run(() =>
            {
                if (!_dbService.CheckUserExists(_username, _password))
                {
                    _loginErrorPopup.ShowWithTimer(5000);
                    UnlockAllControls();
                    return;
                }
                _loginedUser = _dbService.GetUser(_username, _password);
            });

            if(_loginedUser == null)
            {
                _loginErrorPopup.ShowWithTimer(5000);
                UnlockAllControls();
                return;
            }

            SaveLoginedUserToConfigIfNeccessary();
            ChangeCurrentVM(new MainPageViewModel(_navigationStore, _loginedUser, _configManager));
            UnlockAllControls();
        }
        
        private void SaveLoginedUserToConfigIfNeccessary()
        {
            if (_rememberUser)
            {
                _configManager.Config.LoginedUser = _loginedUser;
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

        private bool CheckIfUsernameAndPasswordAreValid()
        {
            return string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password);
        }

        private async void TryToRegister(object parameter)
        {
            BlockAllControls();

            await Task.Run(() => {
                // This delay allows user to enjoy beautiful loader animation UwU
                Task.Delay(1500);
                _canConnectToDB = _dbService.CanBeConnected();
            });

            if (!_canConnectToDB)
            {
                _dbConnectionErrorPopup.ShowWithTimer(5000);
                UnlockAllControls();
                return;
            }

            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (CheckIfUsernameAndPasswordAreValid())
            {
                _registerErrorPopup.ShowWithTimer(5000);
                UnlockAllControls();
                return;
            }

            await Task.Run(() =>
            {
                if (_dbService.AddUser(_username, _password))
                {
                    DisplayRegisterSuccessPopupAndRedirect();
                    UnlockAllControls();
                    return;
                }
                _registerErrorPopup.ShowWithTimer(5000);
                UnlockAllControls();
            });
          
        }
        
        private void DisplayRegisterSuccessPopupAndRedirect()
        {
            _registerSuccessPopup.ShowWithTimer(5000);
            ChangeCurrentVM(new LoginViewModel(_navigationStore, _configManager));
        }

    }
}
