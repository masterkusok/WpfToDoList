using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Stores;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Controls;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {

        private const int ErrorPopupTimer = 5000;

        private string _password = String.Empty;
        private string _passwordRepeat = String.Empty;
        private string _username = String.Empty;

        private bool _controlsEnabled = true;
        private bool _canConnectToDB = false;

        public Dictionary<string, PopupRepresenter> PagePopups { get; set; }
        public string ErrorPopupMessage { get; set; }

        public bool ControlsEnabled { get => _controlsEnabled; set => _controlsEnabled = value; }

        public string Username { get => _username; set => _username = value; }

        private IDbService _dbService;
        private IConfigManager _configManager;

        public ICommand RegisterCommand { get; set; }
        public ICommand GoBackToLoginPageCommand { get; set; }

        public RegisterViewModel(NavigationStore navigationStore, IConfigManager manager)
        {
            PagePopups = new Dictionary<string, PopupRepresenter>();
            SetupPopups();

            ErrorPopupMessage = String.Empty;

            _navigationStore = navigationStore;
            _dbService = new SqlService();
            _configManager = manager;

            _canConnectToDB = _dbService.CanBeConnected();
            RegisterCommand = new RelayCommand(obj =>
            {
                RegisterBtnClicked(obj);
            });
            GoBackToLoginPageCommand = new RelayCommand(obj =>
            {
                ChangeCurrentVM(new LoginViewModel(_navigationStore, _configManager));
            });
        }

        private void SetupPopups()
        {
            PagePopups.Add("LoaderPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("ErrorPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("RegisterSuccessfullyPopup", new PopupRepresenter("PagePopups", this));
        }
        private void GetPasswordFromPasswordBox(object obj)
        {
            object[] boxes = (object[])obj;
            _password = (boxes[0] as PasswordBox).Password;
            _passwordRepeat = (boxes[1] as PasswordBox).Password;
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

        private void OpenLoaderPopup()
        {
            PagePopups["LoaderPopup"].Open();
        }

        private void CloseLoaderPopup()
        {
            PagePopups["LoaderPopup"].Close();
        }

        private async void RegisterBtnClicked(object parameter)
        {
            bool success = false;

            GetPasswordFromPasswordBox(parameter);
            OpenLoaderPopup();
            BlockAllControls();
            await Task.Delay(1500);
            await Task.Run(() =>
            {
                success = TryToRegisterUser();
            });

            CloseLoaderPopup();
            if (success)
            {
                DisplayRegisterSuccessPopupAndRedirect();
                return;
            }
            UnlockAllControls();
        }

        private bool TryToRegisterUser()
        {
            if (CannotConnectDB())
            {
                ShowErrorPopupMessage("Error. Can't connect to server");
                return false;
            }

            if (UsernameAndPasswordAreInvalid())
            {
                ShowErrorPopupMessage("Invalid username or password");
                return false;
            }

            if (_dbService.CheckUserExists(_username, String.Empty))
            {
                ShowErrorPopupMessage("Username is already taken");
                return false;
            }

            if (_password != _passwordRepeat)
            {
                ShowErrorPopupMessage("Passwords do not match");
                return false;
            }

            if(!_dbService.AddUser(_username, _password))
            {
                ShowErrorPopupMessage("Unknown error during register");
                return false;
            }
            return true;
        }

        private bool UsernameAndPasswordAreInvalid()
        {
            return string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password);
        }

        private bool CannotConnectDB()
        {
            return !_dbService.CanBeConnected();
        }

        private async void DisplayRegisterSuccessPopupAndRedirect()
        {
            PagePopups["RegisterSuccessfullyPopup"].ShowWithTimer(ErrorPopupTimer);
            await Task.Delay(ErrorPopupTimer);
            ChangeCurrentVM(new LoginViewModel(_navigationStore, _configManager));
            UnlockAllControls();
        }

    }

}

