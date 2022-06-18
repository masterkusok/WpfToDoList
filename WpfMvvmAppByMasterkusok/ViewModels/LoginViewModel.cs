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
        private bool _canConnectToDB = false;

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
           
            _navigationStore = navigationStore;
            _dbService = new SqlService();
            _configManager = configManager;

            _canConnectToDB = _dbService.CanBeConnected();

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
            PagePopups = new Dictionary<string, PopupRepresenter>();
            PagePopups.Add("LoaderPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("ErrorPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("RegisterSuccessfullyPopup", new PopupRepresenter("PagePopups", this));
        }

        private void GoToRegisterPage()
        {
            ChangeCurrentVM(new RegisterViewModel(_navigationStore, _configManager));
        }

        private async void LoginBtnClicked(object parameter)
        {
            BlockAllControls();
            PagePopups["LoaderPopup"].Open();

            // This delay allows user to enjoy beautiful loader animation UwU
            await Task.Delay(1500);

            await Task.Run(() => {
                _canConnectToDB = _dbService.CanBeConnected();
            });

            if (!_canConnectToDB)
            {
                ShowErrorPopupMessage("Error during connecting to server");
                PagePopups["LoaderPopup"].Close();
                UnlockAllControls();
                return;
            }

            GetPasswordFromPasswordBox((PasswordBox)parameter);
            if (CheckIfUsernameAndPasswordAreValid())
            {
                ShowErrorPopupMessage("Invalid username or password");
                PagePopups["LoaderPopup"].Close();
                UnlockAllControls();
                return;
            }

            await Task.Run(() =>
            {
                if (!_dbService.CheckUserExists(_username, _password))
                {
                    ShowErrorPopupMessage("Can't find such user");
                    PagePopups["LoaderPopup"].Close();
                    UnlockAllControls();
                    return;
                }
                _loginedUser = _dbService.GetUser(_username, _password);
            });

            if(_loginedUser == null)
            {
                ShowErrorPopupMessage("Can't find such user");
                PagePopups["LoaderPopup"].Close();
                UnlockAllControls();
                return;
            }

            SaveLoginedUserToConfigIfNeccessary();
            PagePopups["LoaderPopup"].Close();
            UnlockAllControls();
            ChangeCurrentVM(new MainPageViewModel(_navigationStore, _loginedUser, _configManager));
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
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }

        private bool CheckIfUsernameAndPasswordAreValid()
        {
            return string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password);
        }

    }
}
