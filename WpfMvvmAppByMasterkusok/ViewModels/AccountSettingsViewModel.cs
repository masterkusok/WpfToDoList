using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Windows.Controls;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class AccountSettingsViewModel : BaseViewModel
    {
        private const int ErrorPopupTimer = 5000;
        private User _user;
        private IConfigManager _configManager;
        private string _newPassword;
        private string _newPasswordRepeat;
        public User CurrentUser { get => _user; set => _user = value; }

        public Dictionary<string, PopupRepresenter> PagePopups { get; set; }
        public string ShowingPopupText { get; set; }
        public string NewUsername { get; set; }
        public string NewUsernameRepeat { get; set; }
        public bool ControlsEnabled { get; set; }
        public bool ChangePasswordMode { get; set; }

        public ICommand LogoutCommand { get; set; }
        public ICommand ChangeUsernameCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        private IDbService _dbService;

        public AccountSettingsViewModel(NavigationStore navigationStore, User user, IConfigManager manager)
        {
            _navigationStore = navigationStore;
            _user = user;
            _configManager = manager;
            ControlsEnabled = true;
            _dbService = new SqlService();
            ShowingPopupText = "";
            SetupPopups();
            SetupCommands();
        }

        private void SetupPopups()
        {
            PagePopups = new Dictionary<string, PopupRepresenter>();
            PagePopups.Add("ErrorPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("ChangeUsernamePopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("ChangePasswordPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("LoaderPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("OperationSuccessfullyPopup", new PopupRepresenter("PagePopups", this));
        }

        private void SetupCommands()
        {
            LogoutCommand = new RelayCommand(obj => {
                Logout();
            });

            ChangeUsernameCommand = new RelayCommand(obj => {
                ChangeUsernameButtonPressed();
            });
            ChangePasswordCommand = new RelayCommand(obj =>
            {
                ChangePasswordButtonPressed(obj);
            });
        }

        private void Logout()
        {
            SetNullUserToConfig();
            ChangeCurrentVM(new LoginViewModel(_navigationStore, _configManager));
        }

        private void SetNullUserToConfig()
        {
            _configManager.Config.LoginedUser = null!;
            _configManager.SaveConfiguration();
        }

        private async void ChangeUsernameButtonPressed()
        {
            DisableControls();
            PagePopups["LoaderPopup"].Open();
            await Task.Delay(1500);
            await Task.Run(() =>
            {
                if (!_dbService.CanBeConnected())
                {
                    EnableControls();
                    PagePopups["LoaderPopup"].Close();
                    ShowErrorPopup("Error during conecting to server");
                    return;
                }
            });

            bool userExists = true;
            await Task.Run(() =>
            {
                userExists = _dbService.CheckUserExists(NewUsername, null);
            });

            if (userExists)
            {
                EnableControls();
                PagePopups["LoaderPopup"].Close();
                ShowErrorPopup("Error. Username is already taken");
                return;
            }

            if (string.IsNullOrEmpty(NewUsername) || string.IsNullOrEmpty(NewUsernameRepeat))
            {
                EnableControls();
                PagePopups["LoaderPopup"].Close();
                ShowErrorPopup("Error. You should fill both fields");
                return;
            }

            if (NewUsername != NewUsernameRepeat)
            {
                EnableControls();
                PagePopups["LoaderPopup"].Close();
                ShowErrorPopup("Error. Fields do not match");
                return;
            }

            ChangeUserInDbAndConfig(new User(NewUsername, _user.Password, _user.ToDoItems));
            PagePopups["LoaderPopup"].Close();
            ShowOperationSuccessfullyPopup();
        }

        private void ShowErrorPopup(string message)
        {
            ShowingPopupText = message;
            NotifyOnPropertyChanged(nameof(ShowingPopupText));
            PagePopups["ErrorPopup"].ShowWithTimer(ErrorPopupTimer);
        }

        private void DisableControls()
        {
            ControlsEnabled = false;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
        }

        private void EnableControls()
        {
            ControlsEnabled = true;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            
        }

        private void ChangeUserInDbAndConfig(User newVersion)
        {
            if (_configManager.Config.LoginedUser != null)
            {
                _configManager.Config.LoginedUser = newVersion;
                _configManager.SaveConfiguration();
            }

            _dbService.UpdateUser(_user, newVersion);
            NotifyOnPropertyChanged(nameof(CurrentUser));
        }

        private async void ShowOperationSuccessfullyPopup()
        {
            // Close error popup to avoid display bugs
            PagePopups["ErrorPopup"].Close();

            ShowingPopupText = "Operation executed successfully!";
            NotifyOnPropertyChanged(nameof(ShowingPopupText));
            PagePopups["OperationSuccessfullyPopup"].ShowWithTimer(ErrorPopupTimer);
            await Task.Delay(ErrorPopupTimer);
            EnableControls();
        }
    
        private async void ChangePasswordButtonPressed(object obj)
        {
            GetPasswordsFromBoxes(obj);
            DisableControls();
            PagePopups["LoaderPopup"].Open();
            await Task.Delay(1500);
            await Task.Run(() =>
            {
                if (!_dbService.CanBeConnected())
                {
                    EnableControls();
                    PagePopups["LoaderPopup"].Close();
                    ShowErrorPopup("Error during conecting to server");
                    return;
                }
            });

            if (string.IsNullOrEmpty(_newPassword) || string.IsNullOrEmpty(_newPasswordRepeat))
            {
                EnableControls();
                PagePopups["LoaderPopup"].Close();
                ShowErrorPopup("Error. You should fill both fields");
                return;
            }

            if (_newPassword != _newPasswordRepeat)
            {
                EnableControls();
                PagePopups["LoaderPopup"].Close();
                ShowErrorPopup("Error. Fields do not match");
                return;
            }
            ChangeUserInDbAndConfig(new User(_user.Username, _newPassword, _user.ToDoItems));
            PagePopups["LoaderPopup"].Close();
            ShowOperationSuccessfullyPopup();
        }

        private void GetPasswordsFromBoxes(object obj)
        {
            var values = (object[])obj;
            _newPassword = (values[0] as PasswordBox).Password;
            _newPasswordRepeat = (values[1] as PasswordBox).Password;
        }

    }
}
