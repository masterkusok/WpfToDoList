﻿using System.Collections.ObjectModel;
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
        
        public ICommand CreateNewToDoItemCommand { get; set; }
        public ICommand ToggleToDoItemIsCheckedCommand { get; set; }
        public ICommand DeleteShowingToDoItemCommand { get; set; }
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

            BuildUpCommands();
        }
        
        private void BuildUpCommands()
        {
            CreateNewToDoItemCommand = new RelayCommand(obj =>
            {
                CreateNewToDoItem();
            });

            _toDoItemPopup.OpenCommand = new RelayCommand(obj =>
            {
                _toDoItemPopup.Open();
                GetClickedToDoItemAndNotify(obj);
            });

            ToggleToDoItemIsCheckedCommand = new RelayCommand(obj =>
            {
                ToggleToDoItemIsChecked();
            });

            DeleteShowingToDoItemCommand = new RelayCommand(obj =>
            {
                DeleteShowingToDoItem();
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

        private void GetClickedToDoItemAndNotify(object obj)
        {
            _showingToDoItem = _user.GetToDoItemById((int)obj);
            NotifyOnPropertyChanged(nameof(ShowingToDoItem));
        }

        private void ToggleToDoItemIsChecked()
        {
            if (_showingToDoItem.IsChecked)
            {
                _showingToDoItem.IsChecked = false;
            }
            else
            {
                _showingToDoItem.Check();
            }
            NotifyOnPropertyChanged(nameof(ShowingToDoItem));
        }

        private void DeleteShowingToDoItem()
        {
            ToDoItemPopup.Close();
            RemoveToDoItemFromList(_showingToDoItem);
            UpdateListInDb();
        }

        private void RemoveToDoItemFromList(ToDoItem _toDoItemToRemove)
        {
            _user.RemoveToDoItem(_toDoItemToRemove);
            NotifyOnPropertyChanged(nameof(ToDoList));
        }
    }
}
