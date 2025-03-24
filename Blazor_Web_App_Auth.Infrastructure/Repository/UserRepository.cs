using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Domain.Models;
using Blazor_Web_App_Auth.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;


namespace Blazor_Web_App_Auth.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var user = await _context.Users
                           .Where(u => u.Email == email)
                           .Select(u => new User
                           {
                               Id = u.Id,
                               Name = u.Name,
                               Email = u.Email,
                               Role = u.Role,
                               PasswordHash = u.PasswordHash
                           })
                           .FirstOrDefaultAsync();
            return user!;
        
        }
    }
}
