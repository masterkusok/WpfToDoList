using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Models;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private const int ErrorPopupTimer = 5000;

        private string _password ="";
        private string _username ="";

        private bool _controlsEnabled = true;
        private bool _isRegistrationMode = false;
        private bool _rememberUser = false;
        private bool _canConnectToDB = false;

        private User _loginedUser = null!;

        public Dictionary<string, PopupRepresenter> PagePopups { get; set; }
        public string ErrorPopupMessage { get; set; }

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
            SetupPopups();
           
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

        private void SetupPopups()
        {
            PagePopups = new Dictionary<string, PopupRepresenter>();
            PagePopups.Add("LoaderPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("ErrorPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("RegisterSuccessfullyPopup", new PopupRepresenter("PagePopups", this));
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
                ShowErrorPopupMessage("Error during connecting to server");
                UnlockAllControls();
                return;
            }

            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (CheckIfUsernameAndPasswordAreValid())
            {
                ShowErrorPopupMessage("Invalid username or password");
                UnlockAllControls();
                return;
            }

            await Task.Run(() =>
            {
                if (!_dbService.CheckUserExists(_username, _password))
                {
                    ShowErrorPopupMessage("Can't find such user");
                    UnlockAllControls();
                    return;
                }
                _loginedUser = _dbService.GetUser(_username, _password);
            });

            if(_loginedUser == null)
            {
                ShowErrorPopupMessage("Can't find such user");
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
            PagePopups["LoaderPopup"].Open();
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }

        private void ShowErrorPopupMessage(string message)
        {
            ErrorPopupMessage = message;
            NotifyOnPropertyChanged(nameof(ErrorPopupMessage));
            PagePopups["ErrorPopup"].ShowWithTimer(ErrorPopupTimer);
        }

        private void UnlockAllControls()
        {
            _controlsEnabled = true;
            PagePopups["LoaderPopup"].Close();
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }

        private bool CheckIfUsernameAndPasswordAreValid()
        {
            return string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password);
        }

        private async void TryToRegister(object parameter)
        {
            BlockAllControls();

            await Task.Delay(1500);
            await Task.Run(() => {
                _canConnectToDB = _dbService.CanBeConnected();
            });

            if (!_canConnectToDB)
            {
                ShowErrorPopupMessage("Error during connecting");
                UnlockAllControls();
                return;
            }

            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (CheckIfUsernameAndPasswordAreValid())
            {
                ShowErrorPopupMessage("Invalid username or password");
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
                ShowErrorPopupMessage("Error. Please, try againg later");
                UnlockAllControls();
            });
          
        }
        
        private async void DisplayRegisterSuccessPopupAndRedirect()
        {
            PagePopups["RegisterSuccessfullyPopup"].ShowWithTimer(ErrorPopupTimer);
            await Task.Delay(ErrorPopupTimer);
            ChangeCurrentVM(new LoginViewModel(_navigationStore, _configManager));
        }

    }
}
