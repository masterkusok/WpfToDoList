using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class AccountSettingsViewModel : BaseViewModel
    {
        private const int ErrorPopupTimer = 5000;
        private User _user;
        private IConfigManager _configManager;
        public User CurrentUser { get => _user; set => _user = value; }

        public Dictionary<string, PopupRepresenter> PagePopups { get; set; }
        
        public PopupRepresenter OperationSuccessfullyPopup { get; set; }
        public PopupRepresenter ChangePasswordPopup { get; set; }
        public PopupRepresenter ChangeUsernamePopup { get; set; }
        public PopupRepresenter ErrorPopup { get; set; }
        public PopupRepresenter LoaderPopup { get; set; }
        private string _showingPopupText ="";
        public string ShowingPopupText { get; set; }
        public string NewUsername { get; set; }
        public string NewUsernameRepeat { get; set; }
        public bool ControlsEnabled { get; set; }

        public ICommand LogoutCommand { get; set; }
        public ICommand ChangeUsernameCommand { get; set; }
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
            PagePopups.Add("LoaderPopup", new PopupRepresenter("PagePopups", this));
            PagePopups.Add("OperationSuccessfullyPopup", new PopupRepresenter("PagePopups", this));

            ErrorPopup = new PopupRepresenter(nameof(ErrorPopup), this);
            ChangePasswordPopup = new PopupRepresenter(nameof(ChangePasswordPopup), this);
            ChangeUsernamePopup = new PopupRepresenter(nameof(ChangeUsernamePopup), this);
            LoaderPopup = new PopupRepresenter(nameof(LoaderPopup), this);
            OperationSuccessfullyPopup = new PopupRepresenter(nameof(OperationSuccessfullyPopup), this);
        }

        private void SetupCommands()
        {
            LogoutCommand = new RelayCommand(obj => {
                Logout();
            });

            ChangeUsernameCommand = new RelayCommand(obj => {
                ChangeUsernameButtonPressed();
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
            DisableControlsAndShowLoaderPopup();
            await Task.Delay(1500);
            await Task.Run(() =>
            {
                if (!_dbService.CanBeConnected())
                {
                    EnableControlsAndCloseLoaderPopup();
                    ShowErrorPopup("Error during conecting to server");
                    return;
                }
            });

            if(string.IsNullOrEmpty(NewUsername) || string.IsNullOrEmpty(NewUsernameRepeat))
            {
                EnableControlsAndCloseLoaderPopup();
                ShowErrorPopup("Error. You should fill both fields");
                return;
            }

            if (NewUsername != NewUsernameRepeat)
            {
                EnableControlsAndCloseLoaderPopup();
                ShowErrorPopup("Error. Fields do not match");
                return;
            }
            ChangeUsernameInDbAndConfig();
            EnableControlsAndCloseLoaderPopup();
            ShowOperationSuccessfullyPopup();
        }

        private void ShowErrorPopup(string message)
        {
            ShowingPopupText = message;
            NotifyOnPropertyChanged(nameof(ShowingPopupText));
            PagePopups["ErrorPopup"].ShowWithTimer(ErrorPopupTimer);
        }

        private void DisableControlsAndShowLoaderPopup()
        {
            ControlsEnabled = false;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            PagePopups["LoaderPopup"].Open();
        }

        private void EnableControlsAndCloseLoaderPopup()
        {
            ControlsEnabled = true;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            PagePopups["LoaderPopup"].Close();
        }

        private void ChangeUsernameInDbAndConfig()
        {
            User oldUser = new User(_user.Username, _user.Password, null);
            _user.Username = NewUsername;
            _configManager.Config.LoginedUser = _user;
            _configManager.SaveConfiguration();
            _dbService.UpdateUser(oldUser, _user);
            NotifyOnPropertyChanged(nameof(CurrentUser));
        }

        private void ShowOperationSuccessfullyPopup()
        {
            // Close error popup to avoid display bugs
            PagePopups["ErrorPopup"].Close();

            ShowingPopupText = "Operation executed successfully!";
            NotifyOnPropertyChanged(nameof(ShowingPopupText));
            PagePopups["OperationSuccessfullyPopup"].ShowWithTimer(ErrorPopupTimer);
        }
    }
}
