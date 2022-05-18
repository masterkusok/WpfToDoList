using System.Collections.ObjectModel;

namespace WpfMvvmAppByMasterkusok.Models
{
    class NotExistingUser : User
    {
        public NotExistingUser(string username, string password, ObservableCollection<ToDoItem> toDoItems) : base(username, password, toDoItems)
        {
        }
    }
}
