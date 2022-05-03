using System.Threading.Tasks;

namespace WpfMvvmAppByMasterkusok.Models
{
    interface IDbService
    {
        public User GetUser(string username, string password);
        public bool AddUser(string username, string password);
        public bool DeleteUser(string username, string password);
        public bool UpdateUser(User user);
    }
}
