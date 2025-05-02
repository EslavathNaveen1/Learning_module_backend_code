using Microsoft.EntityFrameworkCore;
using QtechBackend.Interfaces;
using QtechBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QtechBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Users>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<Users> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<Users> AddUserAsync(Users user)
        {
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<Users> UpdateUserAsync(Users user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<Users> PatchUserAsync(int id, Dictionary<string, object> patchData)
        {
            return await _userRepository.PatchUserAsync(id, patchData);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<Users> AuthenticateUserAsync(string email, string password)
        {
            return await _userRepository.AuthenticateUserAsync(email, password);
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _userRepository.RoleExistsAsync(role);
        }


        public async Task<int> GetUserCountAsync()
        {
            return await _userRepository.GetUserCountAsync();
        }


    }
}