using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(string username, string password, string role);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> ValidatePasswordAsync(string password, string passwordHash);
        Task<User> UpdateUserPasswordAsync(string username, string newPassword);
        string HashPassword(string password);
    }
} 