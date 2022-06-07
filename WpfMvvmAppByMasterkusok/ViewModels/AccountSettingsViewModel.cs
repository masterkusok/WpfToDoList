using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;
using WpfMvvmAppByMasterkusok.Commands;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class AccountSettingsViewModel : BaseViewModel
    {
        private User _user;
        private IConfigManager _configManager;
        public User CurrentUser { get => _user; set => _user = value; }

        public PopupRepresenter ChangePasswordPopup { get; set; }
        public PopupRepresenter ChangeUsernamePopup { get; set; }

        public string NewUsername { get; set; }
        public string NewUsernameRepeat { get; set; }

        public ICommand LogoutCommand { get; set; }
        public ICommand ChangeUsernameCommand { get; set; }
        private IDbService _dbService;

        public AccountSettingsViewModel(NavigationStore navigationStore, User user, IConfigManager manager)
        {
            _navigationStore = navigationStore;
            _user = user;
            _configManager = manager;
            _dbService = new SqlService();

            ChangePasswordPopup = new PopupRepresenter(nameof(ChangePasswordPopup), this);
            ChangeUsernamePopup = new PopupRepresenter(nameof(ChangeUsernamePopup), this);

            SetupCommands();
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

        private void ChangeUsernameButtonPressed()
        {
            if(!string.IsNullOrEmpty(NewUsername) && NewUsername == NewUsernameRepeat &&
                !_dbService.CheckUserExists(NewUsername, null))
            {
                User oldUser = new User(_user.Username, _user.Password, null);
                _user.Username = NewUsername;
                _configManager.Config.LoginedUser = _user;
                _configManager.SaveConfiguration();
                _dbService.UpdateUser(oldUser, _user);
            }
        }
    }
}
