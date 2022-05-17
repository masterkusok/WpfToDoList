using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvmAppByMasterkusok.Models
{
    class NotExistingUser : User
    {
        public NotExistingUser(string username, string password, List<ToDoItem> toDoItems) : base(username, password, toDoItems)
        {
        }
    }
}
