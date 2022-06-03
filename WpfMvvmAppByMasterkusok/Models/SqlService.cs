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
                return;
            }
            throw new Exception("Can't load connection string");
        }

        public bool AddUser(string username, string password)
        {
            if(!CheckUsernameIsAlreadyTaken(username))
            {
                return MakeInsertRequestToSql(username, password);
            }
            return false;
        }

        private bool CheckUsernameIsAlreadyTaken(string username)
        {
            return CheckUserExists(username, "");
        }

        private bool MakeInsertRequestToSql(string username, string password)
        {
            OpenConnectionIfNotOpened();

            MySqlCommand command = new MySqlCommand($"INSERT INTO users (username, password, todo_json)" +
            $" VALUES(@Username, @Password, @Json)", _connection);
            command.Parameters.AddWithValue("Username", username);
            command.Parameters.AddWithValue("Password", password);
            command.Parameters.AddWithValue("Json", JsonSerializer.Serialize(new List<ToDoItem>()));
            command.ExecuteNonQuery();

            _connection.Close();
            return true;
        }

        private void OpenConnectionIfNotOpened()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
                return;
            _connection.Open();
        }

        public bool DeleteUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool CheckUserExists(string username, string password)
        {
            OpenConnectionIfNotOpened();
            MySqlCommand command = CreateSelectConcreteUserCommand(username, password);
            MySqlDataReader reader = command.ExecuteReader();
            bool exists = reader.Read();
            reader.Close();
            return exists;
        }

        public User GetUser(string username, string password)
        {
            OpenConnectionIfNotOpened();

            if (CheckUserExists(username, password))
            {
                MySqlCommand command = CreateSelectConcreteUserCommand(username, password);
                User user = TryToReadUsersData(command);
                _connection.Close();
                return user;
            }

            throw new NullReferenceException();
        }

        private MySqlCommand CreateSelectConcreteUserCommand(string username, string password)
        {
            return new MySqlCommand($"SELECT * FROM users WHERE username = '{username}'" +
                    $"{AppendPasswordConditionIfNeeds(password)}", _connection);
        }

        private string AppendPasswordConditionIfNeeds(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "";
            }
            return $"AND password = '{password}'";
        }

        private User TryToReadUsersData(MySqlCommand command)
        {
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            return ConstructUserFromUsersData(reader);
        }

        private User ConstructUserFromUsersData(MySqlDataReader reader)
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

        public bool UpdateUser(User sourceUser, User newVersionOfUser)
        {
            if(CheckUserExists(sourceUser.Username, sourceUser.Password))
            {
                OpenConnectionIfNotOpened();
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
