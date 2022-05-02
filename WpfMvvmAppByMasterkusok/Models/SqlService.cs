using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public bool DeleteUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string username, string password)
        {
            _connection.Open();
            if(_connection.State == System.Data.ConnectionState.Open)
            {
                User user = null;
                MySqlCommand command = new MySqlCommand($"SELECT * FROM todo_users WHERE username = '{username}'" +
                    $"AND password = '{password}'",_connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string json_string = reader["todo_json"].ToString();
                    List<ToDoItem> toDoItems = null;
                    try
                    {
                        toDoItems = JsonSerializer.Deserialize<List<ToDoItem>>(json_string);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    user = new User(username, password, toDoItems);
                    _connection.Close();
                    return user;
                }
            }
            _connection.Close();
            return null;
        }

        public bool UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
