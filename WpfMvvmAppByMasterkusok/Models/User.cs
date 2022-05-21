using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfMvvmAppByMasterkusok.Models
{
    public class User
    {
        private string _username;
        private string _password;
        protected ObservableCollection<ToDoItem> _toDoItems;
        public string Username { get => _username; }
        public string Password { get => _password; }
        public ObservableCollection<ToDoItem> ToDoItems { get => _toDoItems; set => _toDoItems = value; }
        public User(string username, string password, ObservableCollection<ToDoItem> toDoItems)
        {
            _username = username;
            _password = password;
            _toDoItems = toDoItems;
        }
        
        public void AddToDoItem(ToDoItem item)
        {
            item.Id = _toDoItems.Count + 1;
            _toDoItems.Add(item);
        }
        public void RemoveToDoItem(ToDoItem item)
        {
            _toDoItems.Remove(item);
        }
        public ToDoItem GetToDoItemById(int id)
        {
            ToDoItem foundItem = null;
            foreach(ToDoItem item in _toDoItems)
            {
                if(item.Id == id)
                {
                    foundItem = item;
                    break;
                }
            }
            return foundItem;
        }
    }
}
