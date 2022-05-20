using System.Collections.ObjectModel;
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
        public bool IsNewToDoItemEverDay { get => _isNewToDoItemEverDay; set => _isNewToDoItemEverDay = value; }
        public string NewToDoItemText { get => _newToDoItemText; set => _newToDoItemText = value; }
        public ICommand AddToDoItemCommand { get; set; }
        public ICommand CloseAddToDoItemPopupCommand { get; set; }
        public ICommand CreateNewToDoItemCommand { get; set; }
        public bool AddItemPopupOpened { get => _addItemPopupOpened; set => _addItemPopupOpened = value; }

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
            CloseAddToDoItemPopupCommand = new RelayCommand(obj =>{
                CloseAddToDoItemPopup();
            });
            CreateNewToDoItemCommand = new RelayCommand(obj =>{
                CreateNewToDoItem();
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
            }
        }
    }
}
