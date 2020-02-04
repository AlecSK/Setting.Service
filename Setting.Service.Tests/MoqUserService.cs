using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Setting.Service.Application.Helpers;
using Setting.Service.Application.Interfaces;
using Setting.Service.Application.Models;

namespace Setting.Service.Tests
{
    public class MoqUserService : IUserService
    {
        private readonly List<User> _users = new List<User>
        {
            new User {Username = "test", Password = "test" }
        };

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));

            return user?.WithoutPassword();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await Task.Run(() => _users.WithoutPasswords());
        }
    }
}