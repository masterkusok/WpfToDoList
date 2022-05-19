using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using MySql.Data.MySqlClient;

namespace WpfMvvmAppByMasterkusok.Models
{
    internal class SqlService : IDbService
    {
        private string _connectionString;
        MySqlConnection _connection;
        public SqlService()
        {
            LoadConnectionString();
            SetupSqlConnection();
        }

        private void SetupSqlConnection()
        {
            _connection = new MySqlConnection(_connectionString);
        }

        private void LoadConnectionString()
        {
            if(File.Exists("connectionstring.txt"))
            {
                _connectionString = File.ReadAllText("connectionstring.txt");
            }
        }

        public bool AddUser(string username, string password)
        {
            if(GetUser(username, password) is NotExistingUser)
            {
                return MakeInsertRequestToSql(username, password);
            }
            return false;
        }

        private bool MakeInsertRequestToSql(string username, string password)
        {
            _connection.Open();
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                MySqlCommand command = new MySqlCommand($"INSERT INTO users (username, password, todo_json)" +
                    $" VALUES(@Username, @Password, @Json)", _connection);
                command.Parameters.AddWithValue("Username", username);
                command.Parameters.AddWithValue("Password", password);
                command.Parameters.AddWithValue("Json", JsonSerializer.Serialize(new List<ToDoItem>()));
                command.ExecuteNonQuery();
            }
            _connection.Close();
            return true;
        }

        public bool DeleteUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        
        public User GetUser(string username, string password)
        {
            User user = new NotExistingUser("noname", "", new ObservableCollection<ToDoItem>());
            _connection.Open();
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                MySqlCommand command = new MySqlCommand($"SELECT * FROM users WHERE username = '{username}'" +
                    $"AND password = '{password}'", _connection);
                
                user = TryToReadUsersData(command);
            }
            _connection.Close();
            return user;
        }

        private User TryToReadUsersData(MySqlCommand command)
        {
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string json_string = reader["todo_json"].ToString();
                ObservableCollection<ToDoItem> toDoItems = new ObservableCollection<ToDoItem>();
                try
                {
                    toDoItems = JsonSerializer.Deserialize<ObservableCollection<ToDoItem>>(json_string);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return new User(reader["username"].ToString(), reader["password"].ToString(), toDoItems);
            }
            return new NotExistingUser("noname", "", new ObservableCollection<ToDoItem>());
        }
        public bool UpdateUser(User sourceUser, User newVersionOfUser)
        {
            if(GetUser(sourceUser.Username, sourceUser.Password) is not NotExistingUser)
            {
                _connection.Open();
                ExecuteUpdateCommand(sourceUser, newVersionOfUser);
                _connection.Close();
                return true;
            } 
            return false;
        }

        private void ExecuteUpdateCommand(User source, User newVersion)
        {
            MySqlCommand command = new MySqlCommand($"UPDATE users SET username=@Username, password=@Password, todo_json=@Json " +
                $"WHERE username=@OldUsername AND password=@OldPassword"
                , _connection);
            command.Parameters.AddWithValue("Username", newVersion.Username);
            command.Parameters.AddWithValue("Password", newVersion.Password);
            command.Parameters.AddWithValue("Json", 
                JsonSerializer.Serialize<ObservableCollection<ToDoItem>>(newVersion.ToDoItems));
            command.Parameters.AddWithValue("OldUsername", source.Username);
            command.Parameters.AddWithValue("OldPassword", source.Password);
            command.ExecuteNonQuery();
        }
    }
}
