using System;
using System.Text.Json.Serialization;

namespace WpfMvvmAppByMasterkusok.Models
{
    public class ToDoItem
    {
        private bool _everydayTask;
        private string _text;
        private bool _isChecked;
        private DateTime _creationDate;
        public DateTime CreationDate { get => _creationDate; set => _creationDate = value; }
        public bool EverydayTask { get => _everydayTask; set => _everydayTask = value; }
        public bool IsChecked { get => _isChecked; set => _isChecked = value; }
        public string Text { get => _text; set => _text = value; }
        [JsonConstructor]
        public ToDoItem(string text, bool everydayTask, bool isChecked, DateTime creationDate)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            EverydayTask = everydayTask;
            IsChecked = isChecked;
            CreationDate = creationDate;
        }
        public ToDoItem(string text, bool everydayTask)
        {
            _text = text;
            _everydayTask = everydayTask;
            _creationDate = DateTime.Today;
            _isChecked = false;
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
