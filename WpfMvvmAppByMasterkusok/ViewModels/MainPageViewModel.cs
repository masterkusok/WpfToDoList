using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfMvvmAppByMasterkusok.Models;
using WpfMvvmAppByMasterkusok.Stores;

namespace WpfMvvmAppByMasterkusok.ViewModels
{
    internal class MainPageViewModel : BaseViewModel
    {
        private User _user;
        public ObservableCollection<ToDoItem> ToDoList { 
            get=> _user.ToDoItems; 
            set {
                _user.ToDoItems = value;
                NotifyOnPropertyChanged(nameof(ToDoList));
            } 
        }
        public MainPageViewModel(NavigationStore navigationStore, User user)
        {
            _user = user;
            _navigationStore = navigationStore;
        }
    }
}
