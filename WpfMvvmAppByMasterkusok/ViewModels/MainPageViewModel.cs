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
        private bool _addItemPopupOpened;
        private bool _isNewToDoItemEverDay;
        private string _newToDoItemText;
        private ToDoItem _showingToDoItem;
        private bool _toDoItemPopupOpened;
        public bool IsNewToDoItemEverDay { get => _isNewToDoItemEverDay; set => _isNewToDoItemEverDay = value; }
        public string NewToDoItemText { get => _newToDoItemText; set => _newToDoItemText = value; }
        public ICommand AddToDoItemCommand { get; set; }
        public ICommand CloseAddToDoItemPopupCommand { get; set; }
        public ICommand CreateNewToDoItemCommand { get; set; }
        public ICommand OpenToDoItemPageCommand { get; set; }
        public ToDoItem ShowingToDoItem { get => _showingToDoItem; set => _showingToDoItem = value; }
        public bool AddItemPopupOpened { get => _addItemPopupOpened; set => _addItemPopupOpened = value; }
        public bool ToDoItemPopupOpened { get => _toDoItemPopupOpened; set => _toDoItemPopupOpened = value; }

        public ObservableCollection<ToDoItem> ToDoList { 
            get=> _user.ToDoItems; 
            set {
                _user.ToDoItems = value;
                NotifyOnPropertyChanged(nameof(ToDoList));
            } 
        }
        public MainPageViewModel(NavigationStore navigationStore, User user)
        {
            _newToDoItemText = "Text of your task here!";
            _user = user;
            _navigationStore = navigationStore;
            _dbService = new SqlService();
            AddToDoItemCommand = new RelayCommand(obj =>
            {
                DisplayToDoItemCreatingPopup();
            });
            CloseAddToDoItemPopupCommand = new RelayCommand(obj =>
            {
                CloseAddToDoItemPopup();
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
        private void CloseAddToDoItemPopup()
        {
            _addItemPopupOpened = false;
            NotifyOnPropertyChanged(nameof(AddItemPopupOpened));
        }
        private void DisplayToDoItemCreatingPopup()
        {
            _addItemPopupOpened = true;
            NotifyOnPropertyChanged(nameof(AddItemPopupOpened));
        }
        private void CreateNewToDoItem()
        {
            if (!string.IsNullOrEmpty(NewToDoItemText))
            {
                _user.AddToDoItem(new ToDoItem(NewToDoItemText, IsNewToDoItemEverDay));
                NotifyOnPropertyChanged(nameof(ToDoList));
                CloseAddToDoItemPopup();
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
            _showingToDoItem = _user.GetToDoItemById((int)obj);
            _toDoItemPopupOpened = true;
            NotifyOnPropertyChanged(nameof(ToDoItemPopupOpened));
            NotifyOnPropertyChanged(nameof(ShowingToDoItem));
        }
    }
}
