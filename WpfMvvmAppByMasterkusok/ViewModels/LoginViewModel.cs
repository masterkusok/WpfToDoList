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

        private string _password = string.Empty;
        private string _username = string.Empty;

        private bool _controlsEnabled = true;
        private bool _rememberUser = false;

        private User _loginedUser = null!;

        public Dictionary<string, PopupRepresenter> PagePopups { get; set; }
        public string ErrorPopupMessage { get; set; }

        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }
        public bool RememberUser { get => _rememberUser; set => _rememberUser = value; }

        public string Username { get => _username; set => _username = value; }

        private IDbService _dbService;
        private IConfigManager _configManager;

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand GoToRegisterPageCommand { get; set; }

        public LoginViewModel(NavigationStore navigationStore, IConfigManager configManager)
        {
            SetupPopups();
            PagePopups = new Dictionary<string, PopupRepresenter>();

            _navigationStore = navigationStore;
            _dbService = new SqlService();
            _configManager = configManager;

            LoginCommand = new RelayCommand(obj =>
            {
                LoginBtnClicked(obj);
            });
            GoToRegisterPageCommand = new RelayCommand(obj =>
            {
                GoToRegisterPage();
            });
        }

        private void SetupPopups()
        {
            PagePopups.Add("LoaderPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("ErrorPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("RegisterSuccessfullyPopup", new PopupRepresenter("PagePopups", this));
        }

        private void GoToRegisterPage()
        {
            ChangeCurrentVM(new RegisterViewModel(_navigationStore, _configManager));
        }

        private void OpenLoaderPopup()
        {
            PagePopups["LoaderPopup"].Open();
        }

        private void CloseLoaderPopup()
        {
            PagePopups["LoaderPopup"].Close();
        }
        private void BlockAllControls()
        {
            _controlsEnabled = false;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }
        private void UnlockAllControls()
        {
            _controlsEnabled = true;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }

        private void ShowErrorPopupMessage(string message)
        {
            ErrorPopupMessage = message;
            NotifyOnPropertyChanged(nameof(ErrorPopupMessage));
            PagePopups["ErrorPopup"].ShowWithTimer(ErrorPopupTimer);
        }

        private void GetPasswordFromPasswordBox(PasswordBox box)
        {
            _password = box.Password;
        }

        private async void LoginBtnClicked(object parameter)
        {
            bool success = false;

            GetPasswordFromPasswordBox((PasswordBox)parameter);
            OpenLoaderPopup();
            BlockAllControls();
            await Task.Delay(1500);
            await Task.Run(() =>
            {
                success = TryToLogin();
            });

            CloseLoaderPopup();
            if (success)
            {
                SaveLoginedUserToConfigIfNeccessary();
                ChangeCurrentVM(new MainPageViewModel(_navigationStore, _loginedUser, _configManager));
                return;
            }
            UnlockAllControls();
        }
        
        private bool TryToLogin()
        {
            if (CannotConnectDB())
            {
                ShowErrorPopupMessage("Error. Can't connect to server");
                return false;
            }

            if (UsernameOrPasswordAreInvalid())
            {
                ShowErrorPopupMessage("Invalid username or password");
                return false;
            }

            if (UserDoesNotExist())
            {
                ShowErrorPopupMessage("Can't find such user");
                return false;
            }

            _loginedUser = _dbService.GetUser(_username, _password);
            return true;
        }

        private bool CannotConnectDB()
        {
            return !_dbService.CanBeConnected();
        }

        private bool UsernameOrPasswordAreInvalid()
        {
            return string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password);
        }

        private bool UserDoesNotExist()
        {
            return !_dbService.CheckUserExists(Username, string.Empty);
        }

        private void SaveLoginedUserToConfigIfNeccessary()
        {
            if (_rememberUser)
            {
                _configManager.Config.LoginedUser = _loginedUser;
                _configManager.SaveConfiguration();
            }
        }
    }
}
