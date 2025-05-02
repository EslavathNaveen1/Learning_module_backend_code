using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetUsersAsync();
        Task<Users> GetUserByIdAsync(int id);
        Task<Users> AddUserAsync(Users user);
        Task<Users> UpdateUserAsync(Users user);
        Task<Users> PatchUserAsync(int id, Dictionary<string, object> patchData);
        Task DeleteUserAsync(int id);
        Task<Users> AuthenticateUserAsync(string email, string password);
        Task<Users> GetUserByEmailAsync(string email);
        Task<bool> RoleExistsAsync(string role);
        Task<int> GetUserCountAsync();
    }
}