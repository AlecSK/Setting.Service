using System.Collections.Generic;
using System.Threading.Tasks;
using Setting.Service.Application.Models;

namespace Setting.Service.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
    }
}