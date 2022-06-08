namespace WpfMvvmAppByMasterkusok.Models
{
    interface IDbService
    {
        public User GetUser(string username, string password);
        public bool CheckUserExists(string username, string password);
        public bool AddUser(string username, string password);
        public bool DeleteUser(string username, string password);
        public bool UpdateUser(User sourceUser, User newVersionOfUser);
        public bool CanBeConnected();
    }
}
