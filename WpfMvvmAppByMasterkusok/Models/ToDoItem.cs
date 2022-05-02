using System;

namespace WpfMvvmAppByMasterkusok.Models
{
    internal class ToDoItem
    {
        private bool _everydayTask;
        private string _text;
        private bool _isChecked;
        private DateTime _creationDate;
        public bool EverydayTask { get=> _everydayTask; }
        public bool IsChecked { get=> _isChecked; }
        public string Text { get => _text; }
        public ToDoItem(string text, bool everydayTask)
        {
            _text = text;
            _everydayTask = everydayTask;
            _creationDate = DateTime.Today;
            _isChecked = false;
        }
        public ToDoItem(string text, bool everydayTask, bool isChecked, DateTime creationDate)
        {
            _text = text;
            _everydayTask = everydayTask;
            _isChecked = isChecked;
            _creationDate = creationDate;
        }
        public void Check()
        {
            _isChecked = true;
        }
        public void ChangeText(string text)
        {
            _text = text;
        }
        public bool CheckIfOverdue()
        {
            if (EverydayTask)
            {
                return false;
            }
            return _creationDate < DateTime.Today;
        }
    }
}
