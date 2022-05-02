using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvmAppByMasterkusok.Models
{
    internal class User
    {
        private string _username;
        private string _password;
        protected List<ToDoItem> _toDoItems;
        public string Username { get => _username; }
        public string Password { get => _password; }
        public User(string username, string password, List<ToDoItem> toDoItems)
        {
            _username = username;
            _password = password;
            _toDoItems = toDoItems;
        }
        public List<ToDoItem> GetToDoItems()
        {
            return _toDoItems;
        }
        public void AddToDoItem(ToDoItem item)
        {
            _toDoItems.Add(item);
        }
        public void RemoveToDoItem(ToDoItem item)
        {
            _toDoItems.Remove(item);
        }
    }
}
