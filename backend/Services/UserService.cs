using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Entities;
using BCrypt.Net;

namespace SmartNameplate.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> CreateUserAsync(string username, string password, string role)
        {
            var existingUser = await GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"用戶 '{username}' 已存在");
            }

            var user = new User
            {
                UserName = username,
                PasswordHash = HashPassword(password),
                Role = role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<bool> ValidatePasswordAsync(string password, string passwordHash)
        {
            return await Task.Run(() => BCrypt.Net.BCrypt.Verify(password, passwordHash));
        }

        public async Task<User> UpdateUserPasswordAsync(string username, string newPassword)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidOperationException($"用戶 '{username}' 不存在");
            }

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return user;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
} 