using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfMvvmAppByMasterkusok.Commands;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class MainPageViewModel : BaseViewModel
    {

        private IDbService _dbService; 
        private User _user;
        private bool _isNewToDoItemEverDay;
        private string _newToDoItemText;
        private ToDoItem _showingToDoItem;
        private PopupRepresenter _toDoItemPopup;
        private PopupRepresenter _addToDoItemPopup;
        public bool IsNewToDoItemEverDay { get => _isNewToDoItemEverDay; set => _isNewToDoItemEverDay = value; }
        public string NewToDoItemText { get => _newToDoItemText; set => _newToDoItemText = value; }
        public ICommand OpenAddToDoItemPopupCommand { get; set; }
        public ICommand CloseAddToDoItemPopupCommand { get; set; }
        public ICommand CreateNewToDoItemCommand { get; set; }
        public ICommand OpenToDoItemPageCommand { get; set; }
        public ToDoItem ShowingToDoItem { get => _showingToDoItem; set => _showingToDoItem = value; }
        public PopupRepresenter AddToDoItemPopup { get => _addToDoItemPopup; set=> _addToDoItemPopup = value; }
        public PopupRepresenter ToDoItemPopup { get => _toDoItemPopup; set => _toDoItemPopup = value; }

        public ObservableCollection<ToDoItem> ToDoList { 
            get=> _user.ToDoItems; 
            set {
                _user.ToDoItems = value;
                NotifyOnPropertyChanged(nameof(ToDoList));
            } 
        }
        public MainPageViewModel(NavigationStore navigationStore, User user)
        {
            _addToDoItemPopup = new PopupRepresenter(nameof(AddToDoItemPopup), this);
            _toDoItemPopup = new PopupRepresenter(nameof(ToDoItemPopup), this);
            _newToDoItemText = "Text of your task here!";
            _user = user;
            _navigationStore = navigationStore;
            _dbService = new SqlService();
            OpenAddToDoItemPopupCommand = new RelayCommand(obj =>
            {
                _addToDoItemPopup.Open();
            });
            CloseAddToDoItemPopupCommand = new RelayCommand(obj =>
            {
                _addToDoItemPopup.Close();
            });
            CreateNewToDoItemCommand = new RelayCommand(obj =>
            {
                CreateNewToDoItem();
            });
            OpenToDoItemPageCommand = new RelayCommand(obj =>
            {
                OpenToDoItemPage(obj);
            });
        }
        
        private void CreateNewToDoItem()
        {
            if (!string.IsNullOrEmpty(NewToDoItemText))
            {
                _user.AddToDoItem(new ToDoItem(NewToDoItemText, IsNewToDoItemEverDay));
                NotifyOnPropertyChanged(nameof(ToDoList));
                _addToDoItemPopup.Close();
                UpdateListInDb();
            }
        }

        private async void UpdateListInDb()
        {
            await Task.Run(() =>
            {
                _dbService.UpdateUser(_user, _user);
            });
        }

        private void OpenToDoItemPage(object obj)
        {
            _toDoItemPopup.Open();
            _showingToDoItem = _user.GetToDoItemById((int)obj);
            NotifyOnPropertyChanged(nameof(ShowingToDoItem));
        }
    }
}
