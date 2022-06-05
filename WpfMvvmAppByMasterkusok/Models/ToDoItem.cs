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
        private int _id;
        public DateTime CreationDate { get => _creationDate; set => _creationDate = value; }
        public bool EverydayTask { get => _everydayTask; set => _everydayTask = value; }
        public bool IsChecked { get => _isChecked; set => _isChecked = value; }
        public string Text { get => _text; set => _text = value; }
        public int Id { get => _id; set => _id = value; }
        public string FormatedCreationDate
        {
            get => $"Created on {_creationDate.Date.ToShortDateString()}";
        }

        public ToDoItem()
        {

        }

        [JsonConstructor]
        public ToDoItem(string text, bool everydayTask, bool isChecked, DateTime creationDate, int id)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            EverydayTask = everydayTask;
            IsChecked = isChecked;
            CreationDate = creationDate;
            Id = id;
        }
        public ToDoItem(string text, bool everydayTask)
        {
            _text = text;
            _everydayTask = everydayTask;
            _creationDate = DateTime.Today;
            _isChecked = false;
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
