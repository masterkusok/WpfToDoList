using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Commands;
using System.Threading.Tasks;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class AccountSettingsViewModel : BaseViewModel
    {
        private User _user;
        private IConfigManager _configManager;
        public User CurrentUser { get => _user; set => _user = value; }

        public PopupRepresenter ConnectingToDbErrorPopup { get; set; }
        public PopupRepresenter EmptyFieldsErrorPopup { get; set; }
        public PopupRepresenter OperationSuccessfullyPopup { get; set; }
        public PopupRepresenter NotEqualFieldsErrorPopup { get; set; }
        public PopupRepresenter ChangePasswordPopup { get; set; }
        public PopupRepresenter ChangeUsernamePopup { get; set; }
        public PopupRepresenter LoaderPopup { get; set; }

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
            SetupPopups();
            SetupCommands();
        }

        private void SetupPopups()
        {
            ChangePasswordPopup = new PopupRepresenter(nameof(ChangePasswordPopup), this);
            ChangeUsernamePopup = new PopupRepresenter(nameof(ChangeUsernamePopup), this);
            ConnectingToDbErrorPopup = new PopupRepresenter(nameof(ConnectingToDbErrorPopup), this);
            EmptyFieldsErrorPopup = new PopupRepresenter(nameof(EmptyFieldsErrorPopup), this);
            OperationSuccessfullyPopup = new PopupRepresenter(nameof(OperationSuccessfullyPopup), this);
            NotEqualFieldsErrorPopup = new PopupRepresenter(nameof(NotEqualFieldsErrorPopup), this);
            LoaderPopup = new PopupRepresenter(nameof(LoaderPopup), this);
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
            _configManager.Config.LoginedUser = null;
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
                    ConnectingToDbErrorPopup.ShowWithTimer(5000);
                    return;
                }
            });

            if(string.IsNullOrEmpty(NewUsername) || string.IsNullOrEmpty(NewUsernameRepeat))
            {
                EnableControlsAndCloseLoaderPopup();
                EmptyFieldsErrorPopup.ShowWithTimer(5000);
                return;
            }

            if (NewUsername != NewUsernameRepeat)
            {
                EnableControlsAndCloseLoaderPopup();
                NotEqualFieldsErrorPopup.ShowWithTimer(5000);
                return;
            }
            ChangeUsernameInDbAndConfig();
            EnableControlsAndCloseLoaderPopup();
            OperationSuccessfullyPopup.ShowWithTimer(2000);
        }

        private void DisableControlsAndShowLoaderPopup()
        {
            ControlsEnabled = false;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            LoaderPopup.Open();
        }

        private void EnableControlsAndCloseLoaderPopup()
        {
            ControlsEnabled = true;
            NotifyOnPropertyChanged(nameof(ControlsEnabled));
            LoaderPopup.Close();
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
    }
}
